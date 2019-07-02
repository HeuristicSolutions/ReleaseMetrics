using ReleaseMetrics.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReleaseMetrics.Core
{
	public class ApiResult<T> {
		public T Payload { get; set; }
		public List<ResultMsg> Messages { get; set; }

		public bool HasErrors => Messages.Any(x => x.Type == MessageType.Error);

		public ApiResult(T payload, List<ResultMsg> messages = null) {
			Payload = payload;
			Messages = messages ?? new List<ResultMsg>();
		}
    }
}
