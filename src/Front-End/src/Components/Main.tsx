import { memo } from "react";
import { Outlet } from "react-router-dom";
import BottomBar from "./BottomBar";
import TopBar from "./TopBar";
import Transition from "./Transition";

function Main() {
	return (
		<div className="Main SafeArea">
			<TopBar />

			<Transition state>
				<Outlet />
			</Transition>

			<BottomBar />
		</div>
	);
}

export default memo(Main);
