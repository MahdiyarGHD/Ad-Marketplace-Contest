import { memo, useEffect } from "react";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";
import { mainButton } from "@tma.js/sdk-react";

function MyChannels() {
	useEffect(() => {
		// mainButton.setText("Add Your Channel");
		// mainButton.show();
	}, []);

	return (
		<div className="MyChannels">
			<PageHeader>
				<PageHeaderTitle>My Channels</PageHeaderTitle>
			</PageHeader>

			<div className="NoChannel"></div>
		</div>
	);
}

export default memo(MyChannels);
