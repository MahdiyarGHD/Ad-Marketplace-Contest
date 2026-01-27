import { memo } from "react";
import TopBar from "../components/TopBar";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";

function Home() {
	return (
		<div className="Home">
			<PageHeader>
				<PageHeaderTitle>Ad Marketplace</PageHeaderTitle>
			</PageHeader>
		</div>
	);
}

export default memo(Home);
