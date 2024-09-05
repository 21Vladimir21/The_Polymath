using System;
using System.Collections.Generic;
using _Main._Scripts.LetterPooLogic;
using _Main._Scripts.UI.Views;

namespace _Main._Scripts.UI
{
    public class UILocator : IService
    {
        private readonly Dictionary<Type, AbstractView> _uiViews = new();

        private AbstractView _currentView;
        private AbstractView _currentPopupView;

        private AbstractView _lastClosedUI;

        public UILocator(UIViewsHolder uiViewsHolder)
        {
            foreach (var view in uiViewsHolder.Views)
            {
                _uiViews.Add(view.GetType(), view);
                view.Init();
            }
        }

        public TUI GetViewByType<TUI>() where TUI : AbstractView
        {
            if (_uiViews.TryGetValue(typeof(TUI), out var view))
                return view as TUI;
            return null;
        }

        #region Надобы переписать , но пока в это смысла нет

        public void OpenLastUI(Action openedCallback = null, Action closedCallback = null)
        {
            _currentView?.Close(closedCallback);

            _currentView = _lastClosedUI;
            _lastClosedUI.Open(openedCallback);
        }

        public void OpenUI(Type type, Action openedCallback = null, Action closedCallback = null)
        {
            _lastClosedUI = _currentView;
            _currentView?.Close(closedCallback);

            if (TryGetView(type, out var view))
            {
                _currentView = view;
                view.Open(openedCallback);
            }
        }

        private bool TryGetView(Type type, out AbstractView abstractView)
        {
            if (_uiViews.TryGetValue(type, out var view))
            {
                abstractView = view;
                return true;
            }

            abstractView = null;
            return false;
        }

        #endregion
    }
}