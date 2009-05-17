/*
 * Created by SharpDevelop.
 * User: lextm
 * Date: 2008/8/3
 * Time: 15:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Lextm.SharpSnmpLib.Security;

namespace Lextm.SharpSnmpLib.Messaging
{
    /// <summary>
    /// INFORM request message.
    /// </summary>
    public class InformRequestMessage : ISnmpMessage
    {
        private readonly VersionCode _version;
        private readonly IList<Variable> _variables;
        private readonly byte[] _bytes;
        private readonly OctetString _community;
        private readonly ISnmpPdu _pdu;
        private readonly int _requestId;

        /// <summary>
        /// Creates a <see cref="InformRequestMessage"/> with all contents.
        /// </summary>
        /// <param name="requestId">The request id.</param>
        /// <param name="version">Protocol version.</param>
        /// <param name="community">Community name.</param>
        /// <param name="enterprise">Enterprise.</param>
        /// <param name="time">Time ticks.</param>
        /// <param name="variables">Variables.</param>
        [CLSCompliant(false)]
        public InformRequestMessage(int requestId, VersionCode version, OctetString community, ObjectIdentifier enterprise, uint time, IList<Variable> variables)
        {
            _version = version;
            _community = community;
            _variables = variables;
            InformRequestPdu pdu = new InformRequestPdu(new Integer32(requestId), enterprise, new TimeTicks(time), _variables);
            _requestId = requestId;
            _bytes = MessageFactory.PackMessage(_version, _community, pdu).ToBytes();
        }
        
        /// <summary>
        /// Creates a <see cref="InformRequestMessage"/> with a specific <see cref="Sequence"/>.
        /// </summary>
        /// <param name="body">Message body</param>
        public InformRequestMessage(Sequence body)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }
            
            if (body.Count != 3)
            {
                throw new ArgumentException("wrong message body");
            }
            
            _community = (OctetString)body[1];
            _version = (VersionCode)((Integer32)body[0]).ToInt32();
            _pdu = (ISnmpPdu)body[2];
            if (_pdu.TypeCode != SnmpType.InformRequestPdu)
            {
                throw new ArgumentException("wrong message type");
            }
            
            InformRequestPdu pdu = (InformRequestPdu)_pdu;
            _requestId = pdu.RequestId.ToInt32();
            _variables = _pdu.Variables;
            _bytes = body.ToBytes();
        }
        
        /// <summary>
        /// Variables.
        /// </summary>
        public IList<Variable> Variables
        {
            get
            {
                return _variables;
            }
        }

        /// <summary>
        /// Sends the response.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        public void SendResponse(IPEndPoint receiver)
        {
            if (receiver == null)
            {
                throw new ArgumentNullException("receiver");
            }
            
            // TODO: make more efficient here.
            InformRequestPdu pdu = (InformRequestPdu)_pdu;
            new GetResponseMessage(_requestId, _version, _community, pdu.AllVariables).Send(receiver);
        }

        /// <summary>
        /// Sends this <see cref="InformRequestMessage"/> and handles the response from receiver (managers or agents).
        /// </summary>
        /// <param name="timeout">Timeout.</param>
        /// <param name="receiver">Receiver.</param>
        /// <returns></returns>
        public ISnmpMessage GetResponse(int timeout, IPEndPoint receiver)
        {
            return MessageFactory.GetResponse(receiver, _bytes, RequestId, timeout, new UserRegistry(), Messenger.GetSocket(receiver));
        }

        /// <summary>
        /// Sends this <see cref="InformRequestMessage"/> and handles the response from receiver (managers or agents).
        /// </summary>
        /// <param name="timeout">Timeout.</param>
        /// <param name="receiver">Receiver.</param>
        /// <param name="socket">The socket.</param>
        /// <returns></returns>
        public ISnmpMessage GetResponse(int timeout, IPEndPoint receiver, Socket socket)
        {
            return MessageFactory.GetResponse(receiver, _bytes, RequestId, timeout, new UserRegistry(), socket);
        }        
        
        internal int RequestId
        {
            get
            {
                return _requestId;
            }
        }
        
        /// <summary>
        /// Converts to byte format.
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            return _bytes;
        }

        /// <summary>
        /// PDU.
        /// </summary>
        public ISnmpPdu Pdu
        {
            get
            {
                return _pdu;
            }
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public SecurityParameters Parameters
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        /// <value>The scope.</value>
        public Scope Scope
        {
            get { return null; }
        }
        
        /// <summary>
        /// Returns a <see cref="string"/> that represents this <see cref="InformRequestMessage"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "INFORM request message: version: " + _version + "; " + _community + "; " + _pdu;
        }
    }
}
