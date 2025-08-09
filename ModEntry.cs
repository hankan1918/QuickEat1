using System;
using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus; // DialogueBox, Response
using SObject = StardewValley.Object;

namespace QuickEat
{
    internal sealed class ModEntry : Mod
    {
        private ModConfig config = new();

        public override void Entry(IModHelper helper)
        {
            // 설정 파일을 로드한다.
            this.config = helper.ReadConfig<ModConfig>();

            // 키 입력 이벤트 구독
            helper.Events.Input.ButtonPressed += OnButtonPressed;

            // 먹기 확인창 자동으로 Yes 선택
            helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return; // 저장 로드 전 무시.
            if (e.Button != this.config.EatKey) return; // 지정 키가 아니면 무시
            if (this.config.IgnoreWhenMenuOpen && Game1.activeClickableMenu != null) return;
            if (this.config.RequirePlayerFree && !Game1.player.CanMove) return;

            var player = Game1.player;
            // 현재 툴바에서 선택된 아이템
            SObject? heldObj = player.CurrentItem as SObject ?? player.ActiveObject;

            if (heldObj is null)
            {
                this.Monitor.Log("선택된 아이템이 없습니다.");
                return;
            }

            // 섭취 가능한지 판정: edibility == -300 => 비식용
            // (위키 기준) 0은 먹을 수 있으나 수치 변화 없음, 음수 (-299 ~ -1)는 체력 / 기력 감소도 가능.
            if (heldObj.Edibility == -300)
            {
                this.Monitor.Log($"섭취 불가능한 아이템: {heldObj.DisplayName}");
                return;
            }

            // 이미 먹는 중이거나 대기 상태일 때는 중복 호출 방지
            // (SpaceCore 문서 등에서 player.itemToEat)
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var itemToEatField = typeof(Farmer).GetField("itemToEat", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (itemToEatField?.GetValue(player) != null)
            {
                this.Monitor.Log("이미 섭취 중 상태입니다.", LogLevel.Trace);
                return;
            }

            // 먹기 시도전 스택 기록하기
            int beforeStack = heldObj.Stack;

            // 바닐라 '먹기' 루틴을 그대로 호출
            bool didEat = TryInvokeVanillaEat(player, heldObj);

            if (!didEat)
            {
                this.Monitor.Log("먹기 호출에 실패했습니다. 게임 / SMAPI 버전을 확인해주세요.", LogLevel.Trace);
            }
        }

        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            if (e.NewMenu is DialogueBox db)
            {
                var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

                // 현재 '먹기 대기' 상태인지 확인
                var itemToEatField = typeof(Farmer).GetField("itemToEat", flags);
                var toEat = itemToEatField?.GetValue(Game1.player) as SObject;
                if (toEat == null) return;

                try
                {
                    // DialogueBox.responses: List<Response>
                    var responsesField = typeof(DialogueBox).GetField("responses", flags);
                    var responses = responsesField?.GetValue(db) as System.Collections.Generic.List<Response>;
                    if (responses == null || responses.Count == 0) return;

                    Response? yes = null;
                    foreach (var r in responses)
                    {
                        if (string.Equals(r.responseKey, "Yes", StringComparison.OrdinalIgnoreCase))
                        { yes = r; break; }
                    }
                    if (yes == null) return;

                    // answerDialogue(Response) -> 자동 yes
                    var answerMethod = typeof(DialogueBox).GetMethod("answerDialogue", flags, null
                    , new[] { typeof(Response) }, null);
                    answerMethod?.Invoke(db, new object[] { yes });

                    this.Monitor.Log($"먹기 자동 확인: {toEat.DisplayName}", LogLevel.Info);
                }
                catch (Exception ex)
                {
                    this.Monitor.Log($"먹기 자동 확인 실패: {ex}", LogLevel.Warn);
                }
            }
        }

        /// <summary>
        /// 바닐라 메서드를 리플렉션으로 호출한다.
        /// </summary>a
        private bool TryInvokeVanillaEat(Farmer player, SObject food)
        {
            try
            {
                var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                var farmerType = typeof(Farmer);

                // tryToEat(Object) -> bool
                var tryToEat = farmerType.GetMethod("tryToEat", flags, null, new[] { typeof(SObject) }, null);
                if (tryToEat != null)
                {
                    var ok = tryToEat.Invoke(player, new object[] { food }) as bool?;
                    if (ok == true)
                    {
                        this.Monitor.Log($"먹기 실행(tryToEat): {food.DisplayName}", LogLevel.Info);
                        return true;
                    }
                }

                // eatObject(Object)
                var m1 = farmerType.GetMethod("eatObject", flags, null, new[] { typeof(SObject) }, null);
                if (m1 != null)
                {
                    m1.Invoke(player, new object[] { food });
                    this.Monitor.Log($"먹기 실행: {food.DisplayName}", LogLevel.Info);
                    return true;
                }

                // eatObject(Object, bool)
                var m2 = farmerType.GetMethod("eatObject", flags, null, new[] { typeof(SObject), typeof(bool) }, null);
                if (m2 != null)
                {
                    m2.Invoke(player, new object[] { food, false /* 기본 동작 유지 */ });
                    this.Monitor.Log($"먹기 실행(overrideFullness=false): {food.DisplayName}", LogLevel.Info);
                    return true;
                }
            }
            catch (TargetInvocationException tex)
            {
                this.Monitor.Log($"먹기 호출 중 예외: {tex.InnerException?.Message}", LogLevel.Error);
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"먹기 호출 실패: {ex}", LogLevel.Error);
            }

            return false;
        }
    }
}