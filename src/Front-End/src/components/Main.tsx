import { memo, useEffect } from "react";
import { Outlet } from "react-router-dom";
import BottomBar from "./BottomBar";
import TopBar from "./TopBar";
import Transition from "./Transition";
import Toasts from "./Toasts";
import useUIStore from "../stores/useUIStore";
import MainButton from "./MainButton";
import { useShallow } from "zustand/shallow";

function Main({ bottomBarVisible = true }: { bottomBarVisible?: boolean }) {
	const mainButton = useUIStore(useShallow((state) => state.mainButton));

	useEffect(() => {
		return () => {
			useUIStore.setState({
				mainButton: undefined,
			});
		};
	}, []);

	return (
		<div className="Main SafeArea">
			<TopBar />

			<Transition state className="Content">
				<Outlet />
			</Transition>

			<Toasts />

			{mainButton && (
				<MainButton text={mainButton.text} onClick={mainButton.onClick!} />
			)}

			{bottomBarVisible && <BottomBar />}
		</div>
	);
}

export default memo(Main);
