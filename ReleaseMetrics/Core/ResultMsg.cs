using ReleaseMetrics.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReleaseMetrics.Core
{
	public enum MessageType { Error, Warning }

	public class ResultMsg {
		public string Message { get; set; }
		public MessageType Type { get; set; }

		public ResultMsg(string message, MessageType type = MessageType.Warning) {
			Message = message;
			Type = type;
		}
    }
}
