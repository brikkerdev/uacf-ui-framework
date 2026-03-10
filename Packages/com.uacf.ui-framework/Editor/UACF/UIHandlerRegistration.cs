using System;
using System.Reflection;
using UnityEditor;
using UACF.Core;
using UACF.UI.Editor.UACF.Handlers;

namespace UACF.UI.Editor.UACF
{
    [InitializeOnLoad]
    public static class UIHandlerRegistration
    {
        static UIHandlerRegistration()
        {
            EditorApplication.delayCall += () => EditorApplication.delayCall += RegisterHandlers;
        }

        private static int _retryCount;

        private static void RegisterHandlers()
        {
            try
            {
                var router = GetRouter();
                if (router == null)
                {
                    if (++_retryCount < 10)
                        EditorApplication.delayCall += RegisterHandlers;
                    else
                        UnityEngine.Debug.LogWarning("[UACF UI] Could not get RequestRouter. UACF may not be installed or not yet initialized.");
                    return;
                }

                UISetupHandler.Register(router);
                UIThemeHandler.Register(router);
                UITokenHandler.Register(router);
                UIScreenHandler.Register(router);
                UIElementHandler.Register(router);
                UIStyleHandler.Register(router);
                UILayoutHandler.Register(router);
                UIComponentListHandler.Register(router);
                UIBatchHandler.Register(router);

                UnityEngine.Debug.Log("[UACF UI] UI handlers registered successfully.");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning($"[UACF UI] Failed to register handlers: {ex.Message}");
            }
        }

        private static RequestRouter GetRouter()
        {
            try
            {
                var server = UACFBootstrap.GetServer();
                if (server == null) return null;

                var routerField = server.GetType().GetField("_router", BindingFlags.NonPublic | BindingFlags.Instance);
                return routerField?.GetValue(server) as RequestRouter;
            }
            catch
            {
                return null;
            }
        }
    }
}
