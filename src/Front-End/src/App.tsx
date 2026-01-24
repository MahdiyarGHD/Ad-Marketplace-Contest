import { useEffect } from 'react'
import './App.scss'
import { backButton, init, isTMA, miniApp, on, retrieveLaunchParams, themeParams, viewport } from '@tma.js/sdk-react';
import Routes from './Routes';

function App() {
  const handleTheme = (isDark: boolean) => {
    document.body.setAttribute("data-theme", isDark ? "dark" : "light");
  }

  const initializeTMA = async () => {
    if (isTMA()) {
      init()

      const lp = retrieveLaunchParams()

      const platform = lp.tgWebAppPlatform

      if (
        viewport.mount.isAvailable() &&
        !viewport.isMounted()
      ) {
        await viewport.mount();

        viewport.expand()

        if (viewport.requestFullscreen.isAvailable() &&
          (platform === 'ios' || platform === 'android'))
          await viewport.requestFullscreen();

        viewport.bindCssVars()
      }

      if (!miniApp.isMounted()) {
        miniApp.mount()

        miniApp.ready()

        handleTheme(miniApp.isDark())
      }

      if (!themeParams.isMounted()) {
        themeParams.mount()
        themeParams.bindCssVars()
      }

      if (backButton.mount.isAvailable())
        backButton.mount()
    }
  }

  useEffect(() => {
    initializeTMA();

    document.addEventListener("contextmenu", (event) => {
      event.preventDefault();
    });

    // handleTheme(true)

    on('theme_changed', () => handleTheme(miniApp.isDark()))

    return () => {
      if (viewport.isMounted()) {
        // viewport.unmount();
      }

      if (miniApp.isMounted()) {
        miniApp.unmount();
      }

      if (themeParams.isMounted()) {
        themeParams.unmount();
      }
    };
  }, []);

  return <div className="App">
      <Routes />
    </div>
}

export default App