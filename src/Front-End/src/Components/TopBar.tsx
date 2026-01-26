import { memo, useState } from "react";
import TextTransition from "./TextTransition";
import { SearchIcon } from "lucide-react";

function TopBar() {
	const [fetched, setFetched] = useState(false);

	return (
		<div className="TopBar">
			<div className="AppName">
				<div className="title">
					<TextTransition
						text={fetched ? "Ad Marketplace" : "Fetching Data..."}
					/>
				</div>
			</div>
			<div className="Buttons">
				<div className="Search">
					<SearchIcon />
				</div>
			</div>
		</div>
	);
}

export default memo(TopBar);
