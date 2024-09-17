
using System;
using _Main._Scripts.Services;
using _Main._Scripts.UI.Views;
using Random = UnityEngine.Random;

namespace _Main._Scripts.UI.InfoPanel
{
    public class InfoPanelHandler
    {
        private readonly InfosHolder _infos;
        private readonly StepInfoPanel _stepInfoPanel;

        public InfoPanelHandler(InfosHolder infos)
        {
            _infos = infos;
            var uiLocator = ServiceLocator.Instance.GetServiceByType<UILocator>();
            _stepInfoPanel = uiLocator.GetViewByType<StepInfoPanel>();
        }

        public void ShowPanelWithInfo(InfoKey key,Action callback = null) => _stepInfoPanel.SetInfoAndShowPanel(GetInfoFromKey(key),callback);

        public void ShowPanelWithKeyRange(InfoKey firstKey,InfoKey secondKey,Action callback = null)
        {
            var values = Enum.GetValues(typeof(InfoKey));
            var randomKey = Random.Range((int)firstKey, (int)secondKey);
            var key = (InfoKey)values.GetValue(randomKey);
            _stepInfoPanel.SetInfoAndShowPanel(GetInfoFromKey(key), callback);
        }

        private string GetInfoFromKey(InfoKey key)
        {
            foreach (var holder in _infos.Holders)
                if (holder.Key.Equals(key))
                    return holder.InfoStr;

            return default;
        }
    }
}