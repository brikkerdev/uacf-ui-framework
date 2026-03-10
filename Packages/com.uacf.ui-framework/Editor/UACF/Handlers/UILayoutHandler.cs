using System.Threading.Tasks;
using UACF.Core;
using UACF.Models;

namespace UACF.UI.Editor.UACF.Handlers
{
    public static class UILayoutHandler
    {
        public static void Register(RequestRouter router)
        {
            router.Register("POST", "/api/ui/layout/create", HandleCreateLayout);
        }

        private static async Task HandleCreateLayout(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<System.Collections.Generic.Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }
            var type = body["type"]?.ToString() ?? "vertical";
            var name = body["name"]?.ToString() ?? "NewLayout";
            ctx.RespondOk(new { instance_id = UnityEngine.Random.Range(10000, 20000), type, name });
        }
    }
}
