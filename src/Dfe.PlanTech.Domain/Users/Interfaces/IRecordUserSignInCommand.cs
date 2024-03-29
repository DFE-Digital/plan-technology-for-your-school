using Dfe.PlanTech.Domain.SignIns.Models;
using Dfe.PlanTech.Domain.Users.Models;

namespace Dfe.PlanTech.Domain.Users.Interfaces;

public interface IRecordUserSignInCommand
{
    Task<SignIn> RecordSignIn(RecordUserSignInDto recordUserSignInDto);
}
