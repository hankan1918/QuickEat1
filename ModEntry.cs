using System;
using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
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
            }

            // 이미 먹는 중이거나 대기 상태일 때는 중복 호출 방지
            // (SpaceCore 문서 등에서 player.itemToEat)
            var itemToEatField = typeof(Farmer).GetField("itemToEat", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (itemToEatField?.GetValue(player) != null)
            {
                this.Monitor.Log("이미 섭취 중 상태입니다.", LogLevel.Trace);
                return;
            }

            // 바닐라 '먹기' 루틴을 그대로 호출
            if (!TryInvokeVanillaEat(player, heldObj))
            {
                this.Monitor.Log("먹기 호출에 실패했습니다. 게임 / SMAPI 버전을 확인해주세요.", LogLevel.Trace);
            }
        }

        /// <summary>
        /// 바닐라 Farmer.eatObject(...) 메서드를 리플렉션으로 호출한다.
        /// 1.6 계열에서 시그니처가 달라도 대응하도록 1개 / 2개 인수(overrideFullness) 모두 시도.
        /// </summary>
        private bool TryInvokeVanillaEat(Farmer player, SObject food)
        {
            try
            {
                var farmerType = typeof(Farmer);

                // 우선 (Object) 시그니처 시도
                var m1 = farmerType.GetMethod("eatObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(SObject) }, null);
                if (m1 != null)
                {
                    m1.Invoke(player, new object[] { food });
                    this.Monitor.Log($"먹기 실행: {food.DisplayName}", LogLevel.Info);
                    return true;
                }

                // 다음 (Object, bool overrideFullness) 시도 (오버로드 대응)
                var m2 = farmerType.GetMethod("eatObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(SObject), typeof(bool) }, null);
                if (m2 != null)
                {
                    m2.Invoke(player, new object[] { food, false /* 기본 동작 유지 */ });
                    this.Monitor.Log($"먹기 실행(overrideFullness=false): {food.DisplayName}", LogLevel.Info);
                    return true;
                }

                // 다른 경로: tryToEat / performUseAction 등 이름이 다른 경우 대비(드물지만)
                var alt = farmerType.GetMethod("tryToEat", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(SObject) }, null);
                if (alt != null)
                {
                    var ok = alt.Invoke(player, new object[] { food }) as bool?;
                    if (ok == true)
                    {
                        this.Monitor.Log($"먹기 실행(tryToEat): {food.DisplayName}", LogLevel.Info);
                        return true;
                    }
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