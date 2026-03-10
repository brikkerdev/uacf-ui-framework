using System.Threading.Tasks;
using UACF.Core;
using UACF.Models;

namespace UACF.UI.Editor.UACF.Handlers
{
    public static class UIBatchHandler
    {
        public static void Register(RequestRouter router)
        {
            router.Register("POST", "/api/ui/batch", HandleBatch);
        }

        private static async Task HandleBatch(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<System.Collections.Generic.Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }
            ctx.RespondOk(new { success = true, results = new object[0] });
        }
    }
}
