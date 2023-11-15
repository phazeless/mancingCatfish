using System;
using System.Net;

namespace SimpleFirebaseUnity
{
	public class FirebaseError : Exception
	{
		public FirebaseError(HttpStatusCode status, string message) : base(message)
		{
			this.m_Status = status;
		}

		public FirebaseError(HttpStatusCode status, string message, Exception inner) : base(message, inner)
		{
			this.m_Status = status;
		}

		public FirebaseError(string message) : base(message)
		{
		}

		public FirebaseError(string message, Exception inner) : base(message, inner)
		{
		}

		public static FirebaseError Create(WebException webEx)
		{
			HttpStatusCode httpStatusCode = (HttpStatusCode)0;
			bool flag = false;
			if (webEx.Status == WebExceptionStatus.ProtocolError)
			{
				HttpWebResponse httpWebResponse = webEx.Response as HttpWebResponse;
				if (httpWebResponse != null)
				{
					httpStatusCode = httpWebResponse.StatusCode;
					flag = true;
				}
			}
			if (!flag)
			{
				return new FirebaseError(webEx.Message, webEx);
			}
			string message;
			switch (httpStatusCode)
			{
			case HttpStatusCode.BadRequest:
				message = "Firebase request has invalid child names or invalid/missing/too large data";
				break;
			case HttpStatusCode.Unauthorized:
				message = "Firebase request's authorization has failed";
				break;
			default:
				if (httpStatusCode != HttpStatusCode.ExpectationFailed)
				{
					message = webEx.Message;
				}
				else
				{
					message = "Firebase request doesn't specify a Firebase database name";
				}
				break;
			case HttpStatusCode.Forbidden:
				message = "Firebase request violates Firebase Realtime Database Rules";
				break;
			case HttpStatusCode.NotFound:
				message = "Firebase request made over HTTP instead of HTTPS";
				break;
			}
			return new FirebaseError(httpStatusCode, message, webEx);
		}

		public static FirebaseError Create(HttpStatusCode status)
		{
			string message;
			switch (status)
			{
			case HttpStatusCode.BadRequest:
				message = "Firebase request has invalid child names or invalid/missing/too large data";
				break;
			case HttpStatusCode.Unauthorized:
				message = "Firebase request's authorization has failed";
				break;
			default:
				if (status != HttpStatusCode.ExpectationFailed)
				{
					message = "Firebase request's error is not yet documented on Firebase";
				}
				else
				{
					message = "Firebase request doesn't specify a Firebase database name";
				}
				break;
			case HttpStatusCode.Forbidden:
				message = "Firebase request violates Firebase Realtime Database Rules";
				break;
			case HttpStatusCode.NotFound:
				message = "Firebase request made over HTTP instead of HTTPS";
				break;
			}
			return new FirebaseError(status, message);
		}

		public HttpStatusCode Status
		{
			get
			{
				return this.m_Status;
			}
		}

		private const string MESSAGE_ERROR_400 = "Firebase request has invalid child names or invalid/missing/too large data";

		private const string MESSAGE_ERROR_401 = "Firebase request's authorization has failed";

		private const string MESSAGE_ERROR_403 = "Firebase request violates Firebase Realtime Database Rules";

		private const string MESSAGE_ERROR_404 = "Firebase request made over HTTP instead of HTTPS";

		private const string MESSAGE_ERROR_417 = "Firebase request doesn't specify a Firebase database name";

		private const string MESSAGE_ERROR_UNDOCUMENTED = "Firebase request's error is not yet documented on Firebase";

		protected HttpStatusCode m_Status;
	}
}
