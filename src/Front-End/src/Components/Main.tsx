import { memo } from "react";
import { Outlet } from "react-router-dom";
import BottomBar from "./BottomBar";

function Main() {
	return (
		<div className="Main SafeArea">
			<Outlet />

			<BottomBar />
		</div>
	);
}

export default memo(Main);
