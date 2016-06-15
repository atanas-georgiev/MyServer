namespace MyServer.Web.Main
{
    using System.Web;

    using Glimpse.Core.Extensibility;

    using MyServer.Common;

    public class GlimpseSecurityPolicy : IRuntimePolicy
    {
        public RuntimeEvent ExecuteOn => RuntimeEvent.EndRequest | RuntimeEvent.ExecuteResource;

        public RuntimePolicy Execute(IRuntimePolicyContext policyContext)
        {
            return !HttpContext.Current.User.IsInRole(MyServerRoles.Admin) ? RuntimePolicy.Off : RuntimePolicy.On;
        }
    }
}