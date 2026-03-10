using System.Threading.Tasks;
using UACF.Core;
using UACF.Models;

namespace UACF.UI.Editor.UACF.Handlers
{
    public static class UIScreenHandler
    {
        public static void Register(RequestRouter router)
        {
            router.Register("POST", "/api/ui/screen/create", HandleCreateScreen);
            router.Register("GET", "/api/ui/screen/hierarchy", HandleGetHierarchy);
        }

        private static async Task HandleCreateScreen(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<System.Collections.Generic.Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }
            var screenId = body.TryGetValue("screen_id", out var sid) ? sid?.ToString() : null;
            var name = body.TryGetValue("name", out var n) ? n?.ToString() : null;
            var prefabPath = body.TryGetValue("prefab_path", out var pp) ? pp?.ToString() : null;
            ctx.RespondOk(new
            {
                instance_id = UnityEngine.Random.Range(10000, 20000),
                screen_id = screenId ?? "new_screen",
                name = name ?? "NewScreen",
                prefab_path = prefabPath ?? "",
                content_slot_id = 0,
                header_id = 0
            });
        }

        private static async Task HandleGetHierarchy(RequestContext ctx)
        {
            var data = await MainThreadDispatcher.Enqueue(() =>
            {
                return new
                {
                    screen_id = ctx.QueryParams.TryGetValue("screen_id", out var sid) ? sid : "",
                    instance_id = 0,
                    elements = new object[0]
                };
            });
            ctx.RespondOk(data);
        }
    }
}
