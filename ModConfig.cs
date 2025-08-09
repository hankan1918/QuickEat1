using StardewModdingAPI;

namespace QuickEat
{
    public sealed class ModConfig
    {
        /// <summary>음식을 자동 섭취할 단축키</summary>
        public SButton EatKey { get; set; } = SButton.V;

        /// <summary>메뉴가 열려 있을 때는 무시한다.</summary>
        public bool IgnoreWhenMenuOpen { get; set; } = true;

        /// <summary>플레이어가 움직일 수 없을 때는(컷신/이벤트 등) 무시한다.</summary>
        public bool RequirePlayerFree { get; set; } = true;
    }
}