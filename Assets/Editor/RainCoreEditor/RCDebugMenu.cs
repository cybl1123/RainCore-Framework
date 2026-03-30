using RainCoreFramework;
using UnityEditor;
using UnityEngine;
namespace RainCoreFramework
{
    /// <summary>
    /// RainCore 调试等级菜单（单选+勾选）
    /// </summary>
    public static class RCDebugMenu
    {
        // 菜单根路径
        private const string MENU_ROOT1 = "RainCore Framework/DEBUG/DEBUG_LEVEL/Level::";
        private const string MENU_ROOT2 = "RainCore Framework/DEBUG/RainCore/ModuleAllocator/";
        // 存储优先级（保证菜单顺序）
        private const int MENU_PRIORITY = 10;

        // 持久化存储当前选中的调试等级（默认1）
        private static int CurrentDebugLevel
        {
            get => EditorPrefs.GetInt("RainCore_DebugLevel", 1);
            set => EditorPrefs.SetInt("RainCore_DebugLevel", value);
        }

        // ==========================================
        // 1~9 调试等级菜单选项
        // ==========================================
        [MenuItem(MENU_ROOT1 + "1", false, MENU_PRIORITY)]
        public static void DebugLevel1() => SetDebugLevel(1);

        [MenuItem(MENU_ROOT1 + "2", false, MENU_PRIORITY)]
        public static void DebugLevel2() => SetDebugLevel(2);

        [MenuItem(MENU_ROOT1 + "3", false, MENU_PRIORITY)]
        public static void DebugLevel3() => SetDebugLevel(3);

        [MenuItem(MENU_ROOT1 + "4", false, MENU_PRIORITY)]
        public static void DebugLevel4() => SetDebugLevel(4);

        [MenuItem(MENU_ROOT1 + "5", false, MENU_PRIORITY)]
        public static void DebugLevel5() => SetDebugLevel(5);

        [MenuItem(MENU_ROOT1 + "6", false, MENU_PRIORITY)]
        public static void DebugLevel6() => SetDebugLevel(6);

        [MenuItem(MENU_ROOT1 + "7", false, MENU_PRIORITY)]
        public static void DebugLevel7() => SetDebugLevel(7);

        [MenuItem(MENU_ROOT1 + "8", false, MENU_PRIORITY)]
        public static void DebugLevel8() => SetDebugLevel(8);

        [MenuItem(MENU_ROOT1 + "9", false, MENU_PRIORITY)]
        public static void DebugLevel9() => SetDebugLevel(9);

        [MenuItem(MENU_ROOT2 + "PrintDependency")]
        public static bool PrintDependency()
        {
            RainCore.PrintDependency();
            return EditorApplication.isPlaying;
        }
        // ==========================================
        // 核心：设置等级 + 单选勾选逻辑
        // ==========================================
        private static void SetDebugLevel(int level)
        {
            // 1. 保存当前选择
            RCDebug.CurrentDebugLevel = level;

            // 2. 刷新所有菜单的勾选状态（单选互斥）
            for (int i = 1; i <= 9; i++)
            {
                bool isChecked = (i == level);
                Menu.SetChecked(MENU_ROOT1 + i, isChecked);
            }

            RCDebug.LOG($"RainCore 调试等级已切换至：{level}");
        }

        // ==========================================
        // 编辑器启动时自动刷新勾选状态
        // ==========================================
        [InitializeOnLoadMethod]
        private static void AutoRefreshCheckMark()
        {
            int level = RCDebug.CurrentDebugLevel;
            for (int i = 1; i <= 9; i++)
            {
                Menu.SetChecked(MENU_ROOT1 + i, i == level);
            }
        }

        // ==========================================
        // 【运行时读取】给你的框架提供当前等级
        // ==========================================
        public static int GetDebugLevel() => RCDebug.CurrentDebugLevel;
    }
}