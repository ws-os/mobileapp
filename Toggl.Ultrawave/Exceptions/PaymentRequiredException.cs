﻿using System.Net;
using Toggl.Ultrawave.Network;

namespace Toggl.Ultrawave.Exceptions
{
    class PaymentRequiredException : ClientErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.PaymentRequired;

        private const string defaultMessage = "Payment is required for this request.";

        internal PaymentRequiredException(IRequest request, IResponse response)
            : this(request, response, defaultMessage)
        {
        }

        internal PaymentRequiredException(IRequest request, IResponse response, string errorMessage)
            : base(request, response, errorMessage)
        {
        }
    }
}
