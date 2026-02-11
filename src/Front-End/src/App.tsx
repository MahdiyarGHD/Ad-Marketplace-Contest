import { useEffect } from "react";
import "react-day-picker/style.css";
import "./App.scss";
import {
	backButton,
	init,
	isTMA,
	miniApp,
	retrieveLaunchParams,
	retrieveRawInitData,
	themeParams,
	viewport,
} from "@tma.js/sdk-react";
import Routes from "./Routes";
import useAppStore from "./stores/useAppStore";

function App() {
	const { authenticate } = useAppStore();

	const rawInitData = retrieveRawInitData();

	const handleTheme = (isDark: boolean) => {
		document.body.setAttribute("data-theme", isDark ? "dark" : "light");
	};

	const initializeTMA = async () => {
		if (isTMA()) {
			init();

			const lp = retrieveLaunchParams();

			const platform = lp.tgWebAppPlatform;

			if (platform === "ios" || platform === "android") {
				document.body.classList.add("Mobile");
			}

			if (viewport.mount.isAvailable() && !viewport.isMounted()) {
				await viewport.mount();

				viewport.expand();

				if (
					viewport.requestFullscreen.isAvailable() &&
					(platform === "ios" || platform === "android")
				)
					await viewport.requestFullscreen();

				viewport.bindCssVars();
			}

			if (!miniApp.isMounted() && miniApp.mount.isAvailable()) {
				miniApp.mount();

				handleTheme(miniApp.isDark());
				miniApp.isDark.sub(handleTheme);

				miniApp.ready();
			}

			if (!themeParams.isMounted() && themeParams.mount.isAvailable()) {
				themeParams.mount();

				if (!themeParams.isCssVarsBound()) themeParams.bindCssVars();
			}

			// if (!mainButton.isMounted() && mainButton.mount.isAvailable()) {
			// 	mainButton.mount();
			// }

			if (backButton.mount.isAvailable()) backButton.mount();
		}
	};

	const handleAuth = () => {
		authenticate(rawInitData);
	};

	useEffect(() => {
		initializeTMA();

		handleAuth();

		// document.addEventListener("contextmenu", (event) => {
		//   event.preventDefault();
		// });

		// handleTheme(true)

		// on("theme_changed", () => handleTheme(miniApp.isDark()));

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

	return (
		<div className="App">
			<Routes />
		</div>
	);
}

export default App;
