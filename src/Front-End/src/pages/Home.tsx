import { memo } from "react";
import TopBar from "../components/TopBar";

function Home() {
	return (
		<div className="Home">
			<TopBar />
		</div>
	);
}

export default memo(Home);
