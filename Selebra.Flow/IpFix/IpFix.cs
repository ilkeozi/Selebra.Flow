using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selebra.Flow
{
    public class IpFix
    {
        /// <summary>
        /// MessageHeader represents IPFIX message header
        /// </summary>
        /// <remarks>The UInt16 type is not CLS-compliant. The CLS-compliant alternative type is Int32. </remarks>
        public struct MessageHeader
        {
            /// <summary>
            /// Version of IPFIX to which this Message conforms. 
            /// </summary>
            /// <see cref="https://tools.ietf.org/search/rfc3954"/>
            public uint Version; //uint16 // 

            /// <summary>
            /// Total length of the IPFIX Message, measured in octets, including
            ///Message Header and Set(s).
            /// </summary>
            public uint Length; //uint16 // 

            /// <summary>
            /// Time at which the IPFIX Message Header leaves the Exporter,
            /// expressed in seconds since the UNIX epoch of 1 January 1970 at
            ///00:00 UTC, encoded as an unsigned 32-bit integer.
            /// </summary>
            //public uint ExportTime; // uint32 // 
            public DateTime ExportTime;
            /// <summary>
            /// Incremental sequence counter modulo 2^32 of all IPFIX Data Records
            /// sent in the current stream from the current Observation Domain by
            ///the Exporting Process.
            /// </summary>                 
            public uint SequenceNo; //uint32 

            /// <summary>
            /// A 32-bit identifier of the Observation Domain that is locally
            ///unique to the Exporting Process.
            /// </summary>
            public uint DomainID; //uint32 // A 32-bit id that is locally unique to the Exporting Process
        }

        /// <summary>
        /// TemplateHeader represents template fields
        /// </summary>
        public struct TemplateHeader
        {
            public int TemplateID; //uint16 2 bytes max 65536
            public int FieldCount; //uint16 2 bytes max 65536
            public int ScopeFieldCount; //uint16 2 bytes max 65536
        }

        /// <summary>
        /// TemplateRecord represents template records
        /// </summary>
        struct TemplateRecord
        {

            uint TemplateID; //uint16
            uint FieldCount; //uint16
            TemplateFieldSpecifier[] FieldSpecifiers;
            uint ScopeFieldCount; //uint16
            TemplateFieldSpecifier[] ScopeFieldSpecifiers;
        }

        /// <summary>
        /// TemplateFieldSpecifier represents field properties
        /// </summary>
        struct TemplateFieldSpecifier
        {
            uint ElementID; //uint16
            uint Length; //uint16
            uint EnterpriseNo; //uint32
        }

        /// <summary>
        /// Message represents IPFIX decoded data
        /// </summary>
        struct Message
        {

            string AgentID;
            MessageHeader Header;
            DecodedField[][] DataSets;
        }

        /// <summary>
        /// DecodedField represents a decoded field
        /// </summary>
        struct DecodedField
        {
            uint ID; //uint16
            object Value; //interface{ }
            uint EnterpriseNo; //uint32
        }

        /// <summary>
        /// SetHeader represents set header fields
        /// </summary>
        public struct SetHeader
        {
            /// <summary>
            /// Set ID value identifies the Set.  A value of 2 is reserved for the
            ///Template Set.A value of 3 is reserved for the Option Template
            ///Set.  All other values from 4 to 255 are reserved for future use.
            ///Values above 255 are used for Data Sets.  The Set ID values of 0
            ///and 1 are not used for historical reasons [RFC3954].
            /// </summary>
            public int SetID; //uint16 2 bytes max 65536
            public int Length; //uint16 2 bytes max 65536
        }


        /// <summary>
        /// The format of the IPFIX Message Header is shown in Figure F.
        ///0                   1                   2                   3
        ///0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
        ///+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        ///|       Version Number          |            Length             |
        ///+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        ///|                           Export Time                         |
        ///+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        ///|                       Sequence Number                         |
        ///+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        ///|                    Observation Domain ID                      |
        ///+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        ///               Figure F: IPFIX Message Header Format
        ///Each Message Header field is exported in network byte order.The
        ///fields are defined as follows:
        /// </summary>
        private MessageHeader DecodeMessageHeader(byte[] bytes)
        {
            MessageHeader header = new MessageHeader();

            header.Version = ToUInt16BigEndian(bytes, 0);
            header.Length = ToUInt16BigEndian(bytes, 2);
            header.ExportTime = (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(ToUInt32BigEndian(bytes, 4));
            header.SequenceNo = ToUInt32BigEndian(bytes, 8);
            header.DomainID = ToUInt32BigEndian(bytes, 12);

            return header;
        }

        //These two functions are from here http://snipplr.com/view/15179/adapt-systembitconverter-to-handle-big-endian-network-byte-ordering-in-order-to-create-number-types-from-bytes-and-viceversa/
        //BitConverter.ToUInt16 would parse the results in "little endian" order so 0x000a would actually be parsed as 0x0a00 and give you 2,560 instead of 10.
        //The spec says that everything should be in "big endian" (also known as "network order"
        public static UInt16 ToUInt16BigEndian(byte[] value, int startIndex)
        {
            return System.BitConverter.ToUInt16(value.Reverse().ToArray(), value.Length - sizeof(UInt16) - startIndex);
        }
        public static UInt32 ToUInt32BigEndian(byte[] value, int startIndex)
        {
            return System.BitConverter.ToUInt32(value.Reverse().ToArray(), value.Length - sizeof(UInt32) - startIndex);
        }
    }
}
