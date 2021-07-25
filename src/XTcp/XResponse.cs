using System;
using System.Collections.Generic;
using System.Text;

namespace XTcp
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/26 18:50:19
    /// description:Response to a synchronous request
    public class XResponse
    { 
        /// <summary>
        /// Metadata to attach to the response.
        /// </summary>
        public Dictionary<object, object> Metadata { get; }

        /// <summary>
        /// Data to attach to the response.
        /// </summary>
        public byte[] Data { get; }
         
        internal DateTime ExpirationUtc { get; set; }

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="req">The synchronous request for which this response is intended.</param>
        /// <param name="data">Data to send as a response.</param>
        public XResponse(XRequest req, string data)
        {
            if (req == null) throw new ArgumentNullException(nameof(req));  
            ExpirationUtc = req.ExpirationUtc;

            Metadata = new Dictionary<object, object>();
            if (String.IsNullOrEmpty(data))
            {
                Data = new byte[0];
            }
            else
            {
                Data = Encoding.UTF8.GetBytes(data);
            }
        }

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="req">The synchronous request for which this response is intended.</param>
        /// <param name="data">Data to send as a response.</param>
        public XResponse(XRequest req, byte[] data)
        {
            if (req == null) throw new ArgumentNullException(nameof(req));
            ExpirationUtc = req.ExpirationUtc;

            Metadata = new Dictionary<object, object>();
            Data = data;
        }

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="req">The synchronous request for which this response is intended.</param>
        /// <param name="metadata">Metadata to attach to the response.</param>
        /// <param name="data">Data to send as a response.</param>
        public XResponse(XRequest req, Dictionary<object, object> metadata, string data)
        {
            if (req == null) throw new ArgumentNullException(nameof(req));
            ExpirationUtc = req.ExpirationUtc;

            Metadata = metadata;

            if (String.IsNullOrEmpty(data))
            {
                Data = new byte[0];
            }
            else
            {
                Data = Encoding.UTF8.GetBytes(data);
            }
        }

        /// <summary>
        /// Instantiate the object.
        /// </summary> 
        /// <param name="req">The synchronous request for which this response is intended.</param>
        /// <param name="metadata">Metadata to attach to the response.</param>
        /// <param name="data">Data to send as a response.</param>
        public XResponse(XRequest req, Dictionary<object, object> metadata, byte[] data)
        {
            if (req == null) throw new ArgumentNullException(nameof(req));
            ExpirationUtc = req.ExpirationUtc;

            Metadata = metadata;
            Data = data;
        }

        internal XResponse(DateTime expirationUtc, Dictionary<object, object> metadata, byte[] data)
        {
            ExpirationUtc = expirationUtc;
            Metadata = metadata;
            Data = data;
        }
    }
}
