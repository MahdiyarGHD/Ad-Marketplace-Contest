import { memo } from "react";
import { Outlet } from "react-router-dom";
import BottomBar from "./BottomBar";
import TopBar from "./TopBar";
import Transition from "./Transition";

function Main({ bottomBarVisible = true }: { bottomBarVisible?: boolean }) {
	return (
		<div className="Main SafeArea">
			<TopBar />

			<Transition state className="Content">
				<Outlet />
			</Transition>

			{bottomBarVisible && <BottomBar />}
		</div>
	);
}

export default memo(Main);
