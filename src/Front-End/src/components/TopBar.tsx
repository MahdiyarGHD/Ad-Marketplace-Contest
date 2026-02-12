import { memo, useEffect, useState } from "react";
import TextTransition from "./TextTransition";
import useUIStore from "../stores/useUIStore";

function TopBar() {
	const [fetched, setFetched] = useState(false);

	const { topBarTitle, topBarButtons } = useUIStore();

	useEffect(() => {
		setFetched(true);
	}, []);

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
			<div className="Buttons">{topBarButtons}</div>
		</div>
	);
}

export default memo(TopBar);
