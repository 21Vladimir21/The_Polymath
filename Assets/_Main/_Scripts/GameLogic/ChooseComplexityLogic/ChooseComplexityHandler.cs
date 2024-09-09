using _Main._Scripts.BotLogic;
using _Main._Scripts.GameDatas;

namespace _Main._Scripts.GameLogic
{
    public class ChooseComplexityHandler
    {
        private readonly ComplexityButton[] _buttons;
        private readonly CurrentGameData _currentGameData;

        public ChooseComplexityHandler(ComplexityButton[] buttons, CurrentGameData currentGameData)
        {
            _buttons = buttons;
            _currentGameData = currentGameData;
            foreach (var button in _buttons)
            {
                button.Init();
                button.OnChooseComplexity += UpdateComplexity;
            }
        }

        public void SetDefaultComplexity() => _buttons[0].Button.onClick.Invoke();

        private void UpdateComplexity(BotComplexity botComplexity)
        {
            foreach (var button in _buttons)
                button.Button.interactable = true;

            _currentGameData.SetComplexity(botComplexity);
        }
    }
}