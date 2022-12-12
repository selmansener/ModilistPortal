using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;
using MediatR;
using Microsoft.Extensions.Hosting;

using ModilistPortal.Business.CQRS.AccountDomain.DTOs;
using ModilistPortal.Data.Repositories.AccountDomain;
using ModilistPortal.Domains.Models.AccountDomain;

namespace ModilistPortal.Business.CQRS.AccountDomain.Commands
{
    public class VerifyAccount : IRequest<VerificationResultDTO>
    {
        public string Email { get; set; }
    }

    internal class VerifyAccountValidator : AbstractValidator<VerifyAccount>
    {
        public VerifyAccountValidator()
        {
            RuleFor(x => x.Email).NotEmpty();
        }
    }

    internal class VerifyAccountHandler : IRequestHandler<VerifyAccount, VerificationResultDTO>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IHostEnvironment _environment;

        public VerifyAccountHandler(IAccountRepository accountRepository, IHostEnvironment environment)
        {
            _accountRepository = accountRepository;
            _environment = environment;
        }

        public async Task<VerificationResultDTO> Handle(VerifyAccount request, CancellationToken cancellationToken)
        {
            Account? account = await _accountRepository.GetByMail(request.Email, cancellationToken);

            string redirectBaseUrl;

            switch (_environment.EnvironmentName)
            {
                case "Development":
                    redirectBaseUrl = "https://localhost:3000";
                    break;
                case "Int":
                    redirectBaseUrl = "https://int.modilist.com";
                    break;
                case "Staging":
                    redirectBaseUrl = "https://staging.modilist.com";
                    break;
                case "Production":
                    redirectBaseUrl = "https://app.modilist.com";
                    break;
                default:
                    throw new Exception("Unknown host enviroment");
            }

            if (account == null)
            {
                return new VerificationResultDTO
                {
                    RedirectUrl = $"{redirectBaseUrl}/verification/account-verification-failed",
                    IsSuccess = false
                };
            }

            account.Verify();

            await _accountRepository.UpdateAsync(account, cancellationToken);

            return new VerificationResultDTO
            {
                RedirectUrl = $"{redirectBaseUrl}/verification/account-verified",
                IsSuccess = true
            };
        }
    }
}
