using System.Threading.Tasks;
using UACF.Core;

namespace UACF.UI.Editor.UACF.Handlers
{
    public static class UIComponentListHandler
    {
        public static void Register(RequestRouter router)
        {
            router.Register("GET", "/api/ui/components/list", HandleListComponents);
        }

        private static async Task HandleListComponents(RequestContext ctx)
        {
            var data = await MainThreadDispatcher.Enqueue(() =>
            {
                var category = ctx.QueryParams.TryGetValue("category", out var c) ? c : "all";
                return new
                {
                    components = new object[]
                    {
                        new { type = "UIButton", category = "input", prefab_variants = new[] { "Filled", "Outlined", "Text", "Tonal" }, properties = new object[0] },
                        new { type = "UIText", category = "display", prefab_variants = new string[0], properties = new object[0] },
                        new { type = "UIVerticalLayout", category = "layout", prefab_variants = new string[0], properties = new object[0] },
                        new { type = "UIPanel", category = "container", prefab_variants = new string[0], properties = new object[0] }
                    }
                };
            });
            ctx.RespondOk(data);
        }
    }
}
