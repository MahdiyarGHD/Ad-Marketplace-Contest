import { memo } from "react";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";

function Deals() {
	return (
		<div className="Deals">
			<PageHeader>
				<PageHeaderTitle>Deals</PageHeaderTitle>
			</PageHeader>
		</div>
	);
}

export default memo(Deals);
