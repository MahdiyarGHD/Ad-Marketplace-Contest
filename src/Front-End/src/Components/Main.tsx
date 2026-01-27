import { memo } from "react";
import { Outlet } from "react-router-dom";
import BottomBar from "./BottomBar";
import TopBar from "./TopBar";

function Main() {
	return (
		<div className="Main SafeArea">
			<TopBar />

			<Outlet />

			<BottomBar />
		</div>
	);
}

export default memo(Main);
