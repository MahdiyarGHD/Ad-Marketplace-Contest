import { memo } from "react";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";
import { ChevronRight } from "lucide-react";
import "./Home.scss";
import Transition from "../components/Transition";

function Home() {
	return (
		<div className="Home">
			<PageHeader>
				<PageHeaderTitle>Ad Marketplace</PageHeaderTitle>
			</PageHeader>

			<div className="Categories Section">
				<div className="title">Categories</div>
				<div className="Items">
					<Transition state eachElement eachElementDelay={20}>
						<div className="Item">
							<div className="icon">😂</div>
							<div className="body">
								<div className="title">Category 1</div>
								<div className="subtitle">Description</div>
							</div>
							<div className="meta">
								{/* <div className="count">12</div> */}
								<ChevronRight />
							</div>
						</div>
					</Transition>
				</div>
			</div>
		</div>
	);
}

export default memo(Home);
