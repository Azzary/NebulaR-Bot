using Nebular.Core.Network.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nebular.IO;
using System.Security.Cryptography;
using Nebular.IO.Interfaces;
using Nebular.Core;

namespace Nebular.Bot
{
    internal class test
    {
        bool DEBUG_LOW_LEVEL_VERBOSE = true;
        private int _id;

        //NetworkMessage
        private void Receive(IDataReader input, bool fromEnterFrame = false)
        {
            NetworkMessage msg = null;
            try
            {
                if (input.BytesAvailable > 0)
                {
                    if (DEBUG_LOW_LEVEL_VERBOSE)
                    {
                        if (fromEnterFrame)
                        {
                            Logger.info("[" + this._id + "] Traitement des données, octets disponibles : " + input.BytesAvailable + " déclenché par une minuterie");
                        }
                        else
                        {
                            Logger.info("[" + this._id + "] Traitement des données, octets disponibles : " + input.BytesAvailable);
                        }
                    }
                    msg = this.LowReceive(input);
                    while (msg != null)
                    {
                        this.Process(msg);
                        msg = this.LowReceive(input);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.StackTrace != null)
                {
                    Logger.info("[" + _id + "] Erreur lors de la lecture de la socket. " + e.StackTrace);
                }
                else
                {
                    Logger.info("[" + _id + "] Erreur lors de la lecture de la socket. Aucune trace de la pile disponible");
                }
            }
        }

        bool _splittedPacket = false;
        protected NetworkMessage LowReceive(IDataReader src)
        {
            NetworkMessage msg = null;
            uint staticHeader = 0;
            uint messageId = 0;
            uint messageLength = 0;
            if (!this._splittedPacket)
            {
                if (src.BytesAvailable < 2)
                {
                    if (DEBUG_LOW_LEVEL_VERBOSE)
                    {
                        Logger.info("[" + this._id + "] Données insuffisantes pour lire l'en-tête, octets disponibles : " + src.BytesAvailable + " (nécessaires : 2)");
                    }
                    return null;
                }
                staticHeader = (uint)src.ReadUShort();
                messageId = this.GetMessageId(staticHeader);
                if (src.BytesAvailable >= (staticHeader & NetworkMessage.BIT_MASK))
                {
                    messageLength = this.ReadMessageLength(staticHeader, src);
                    if (src.BytesAvailable >= messageLength)
                    {
                        this.UpdateLatency();
                        if (this.GetUnpackMode(messageId, messageLength) == UnpackMode.ASYNC)
                        {
                            src.ReadBytes(this._input, this._input.Length, messageLength);
                            msg = this._rawParser.ParseAsync(new CustomDataWrapper(this._input), messageId, messageLength, this.ComputeMessage);
                            if (DEBUG_LOW_LEVEL_VERBOSE && msg != null)
                            {
                                _log.info("[" + this._id + "] Analyse asynchrone " + this.GetType(msg) + ", longueur du message : " + messageLength + ")");
                            }
                        }
                        else
                        {
                            msg = this._rawParser.Parse(new CustomDataWrapper(src), messageId, messageLength);
                            if (DEBUG_LOW_LEVEL_VERBOSE)
                            {
                                _log.info("[" + this._id + "] Analyse complète terminée");
                            }
                        }
                        return msg;
                    }
                    if (DEBUG_LOW_LEVEL_VERBOSE)
                    {
                        _log.info("[" + this._id + "] Données insuffisantes pour lire le contenu du message, octets disponibles : " + src.BytesAvailable + " (nécessaires : " + messageLength + ")");
                    }
                    this._staticHeader = -1;
                    this._splittedPacketLength = messageLength;
                    this._splittedPacketId = messageId;
                    this._splittedPacket = true;
                    src.ReadBytes(this._inputBuffer, 0, src.BytesAvailable);
                    return null;
                }
                if (DEBUG_LOW_LEVEL_VERBOSE)
                {
                    _log.info("[" + this._id + "] Données insuffisantes pour lire l'ID du message, octets disponibles : " + src.BytesAvailable + " (nécessaires : " + (staticHeader & NetworkMessage.BIT_MASK) + ")");
                }
                this._staticHeader = staticHeader;
                this._splittedPacketLength = messageLength;
                this._splittedPacketId = messageId;
                this._splittedPacket = true;
                return null;
            }
            if (this._staticHeader != -1)
            {
                this._splittedPacketLength = this.ReadMessageLength(this._staticHeader, src);
                this._staticHeader = -1;
            }
            if (src.BytesAvailable + this._inputBuffer.Length >= this._splittedPacketLength)
            {
                src.ReadBytes(this._inputBuffer, this._inputBuffer.Length, this._splittedPacketLength - this._inputBuffer.Length);
                this._inputBuffer.Position = 0;
                this.UpdateLatency();
                if (this.GetUnpackMode(this._splittedPacketId, this._splittedPacketLength) == UnpackMode.ASYNC)
                {
                    msg = this._rawParser.ParseAsync(new CustomDataWrapper(this._inputBuffer), this._splittedPacketId, this._splittedPacketLength, this.ComputeMessage);
                    if (DEBUG_LOW_LEVEL_VERBOSE && msg != null)
                    {
                        _log.info("[" + this._id + "] Analyse asynchrone d'un message fractionné " + this.GetType(msg) + ", longueur du message : " + this._splittedPacketLength + ")");
                    }
                }
                else
                {
                    msg = this._rawParser.Parse(new CustomDataWrapper(this._inputBuffer), this._splittedPacketId, this._splittedPacketLength);
                    if (DEBUG_LOW_LEVEL_VERBOSE)
                    {
                        _log.info("[" + this._id + "] Analyse complète terminée");
                    }
                }
                this._splittedPacket = false;
                this._inputBuffer = new ByteArray();
                return msg;
            }
            src.ReadBytes(this._inputBuffer, this._inputBuffer.Length, src.BytesAvailable);
            return null;
        }



    }
}
