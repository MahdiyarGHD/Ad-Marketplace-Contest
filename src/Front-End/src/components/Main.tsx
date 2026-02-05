import { memo } from "react";
import { Outlet } from "react-router-dom";
import BottomBar from "./BottomBar";
import TopBar from "./TopBar";
import Transition from "./Transition";
import Toasts from "./Toasts";

function Main({ bottomBarVisible = true }: { bottomBarVisible?: boolean }) {
	return (
		<div className="Main SafeArea">
			<TopBar />

			<Transition state className="Content">
				<Outlet />
			</Transition>

			<Toasts />

			{bottomBarVisible && <BottomBar />}
		</div>
	);
}

export default memo(Main);
