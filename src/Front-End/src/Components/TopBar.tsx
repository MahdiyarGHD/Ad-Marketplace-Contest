import { memo, useState } from "react";
import TextTransition from "./TextTransition";
import { SearchIcon } from "lucide-react";
import useUIStore from "../stores/useUIStore";

function TopBar() {
	const [fetched, setFetched] = useState(false);

	const { topBarTitle } = useUIStore();

	return (
		<div className="TopBar">
			<div className="AppName">
				<div className="title">
					<TextTransition
						text={
							topBarTitle ?? (fetched ? "Ad Marketplace" : "Fetching Data...")
						}
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
