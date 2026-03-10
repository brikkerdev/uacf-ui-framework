using System.Collections.Generic;
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
            var body = await ctx.ReadBodyAsync<Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }

            if (!body.TryGetValue("operations", out var opsObj))
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "operations array required");
                return;
            }

            var opsList = opsObj as System.Collections.IList ?? (opsObj as object[]);
            if (opsList == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "operations must be an array");
                return;
            }

            var stopOnError = body.TryGetValue("stop_on_error", out var soe) && soe is bool b && b;
            var results = new Dictionary<string, object>();
            var resultsList = new List<object>();

            foreach (var opObj in opsList)
            {
                if (opObj is not Dictionary<string, object> op)
                {
                    if (stopOnError)
                    {
                        ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Each operation must be an object");
                        return;
                    }
                    continue;
                }

                var id = op.TryGetValue("id", out var idVal) ? idVal?.ToString() : null;
                var method = op.TryGetValue("method", out var m) ? m?.ToString()?.ToUpperInvariant() ?? "POST" : "POST";
                var path = op.TryGetValue("path", out var p) ? p?.ToString() : null;
                var opBody = op.TryGetValue("body", out var ob) ? ob as Dictionary<string, object> : null;

                if (string.IsNullOrEmpty(path) || opBody == null)
                {
                    if (stopOnError)
                    {
                        ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Each operation must have path and body");
                        return;
                    }
                    continue;
                }

                var mergedBody = ResolveRefs(new Dictionary<string, object>(opBody), results);

                object result = null;
                try
                {
                    result = await Dispatch(method, path, mergedBody);
                }
                catch (System.Exception ex)
                {
                    if (stopOnError)
                    {
                        ctx.RespondError(500, ErrorCode.INTERNAL_ERROR, ex.Message);
                        return;
                    }
                    resultsList.Add(new { id, error = ex.Message });
                    continue;
                }

                var instanceId = ExtractInstanceId(result);
                if (!string.IsNullOrEmpty(id) && instanceId.HasValue)
                    results[id] = new Dictionary<string, object> { ["instance_id"] = instanceId.Value };

                resultsList.Add(new { id, result, instance_id = instanceId });
            }

            ctx.RespondOk(new { success = true, results = resultsList });
        }

        internal static Dictionary<string, object> ResolveRefs(Dictionary<string, object> body, Dictionary<string, object> results)
        {
            var merged = new Dictionary<string, object>();
            foreach (var kv in body)
            {
                var key = kv.Key;
                var val = kv.Value;

                if (key == "parent" && val is Dictionary<string, object> parentDict &&
                    parentDict.TryGetValue("ref", out var refVal))
                {
                    var refId = refVal?.ToString();
                    if (!string.IsNullOrEmpty(refId) && results.TryGetValue(refId, out var refResult) &&
                        refResult is Dictionary<string, object> refData &&
                        refData.TryGetValue("instance_id", out var instId))
                    {
                        merged[key] = new Dictionary<string, object> { ["instance_id"] = instId };
                        continue;
                    }
                }

                if (val is Dictionary<string, object> nested)
                    merged[key] = ResolveRefs(nested, results);
                else if (val is List<object> list)
                {
                    var newList = new List<object>();
                    foreach (var item in list)
                    {
                        if (item is Dictionary<string, object> itemDict)
                            newList.Add(ResolveRefs(itemDict, results));
                        else
                            newList.Add(item);
                    }
                    merged[key] = newList;
                }
                else
                    merged[key] = val;
            }
            return merged;
        }

        internal static int? ExtractInstanceId(object result)
        {
            if (result is Dictionary<string, object> d)
            {
                if (d.TryGetValue("instance_id", out var id))
                {
                    if (id is int i) return i;
                    if (id is long l) return (int)l;
                    if (id != null && int.TryParse(id.ToString(), out var parsed)) return parsed;
                }
                if (d.TryGetValue("created", out var created) && created is object[] arr && arr.Length > 0 &&
                    arr[0] is Dictionary<string, object> first && first.TryGetValue("instance_id", out var cid))
                {
                    if (cid is int ci) return ci;
                    if (cid is long cl) return (int)cl;
                    if (cid != null && int.TryParse(cid.ToString(), out var cparsed)) return cparsed;
                }
            }
            return null;
        }

        internal static Task<object> Dispatch(string method, string path, Dictionary<string, object> body)
        {
            path = path?.Trim().ToLowerInvariant() ?? "";

            if (method == "POST")
            {
                if (path == "/api/ui/element/add" || path.EndsWith("element/add"))
                    return UIElementHandler.ExecuteAddElement(body);
                if (path == "/api/ui/layout/create" || path.EndsWith("layout/create"))
                    return UILayoutHandler.ExecuteCreateLayout(body);
                if (path == "/api/ui/screen/create" || path.EndsWith("screen/create"))
                    return UIScreenHandler.ExecuteCreateScreen(body);
            }

            if (method == "PUT")
            {
                if (path == "/api/ui/element/modify" || path.EndsWith("element/modify"))
                    return UIElementHandler.ExecuteModifyElement(body);
                if (path == "/api/ui/theme/apply" || path.EndsWith("theme/apply"))
                    return UIThemeHandler.ExecuteApplyTheme(body);
            }

            return Task.FromResult<object>(new { success = false, error = $"Unsupported batch operation: {method} {path}" });
        }
    }
}
