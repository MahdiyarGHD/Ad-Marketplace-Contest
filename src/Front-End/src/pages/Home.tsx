import { memo, useState } from "react";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";
import { ChevronRight } from "lucide-react";
import "./Home.scss";
import Transition from "../components/Transition";
import Tabs, { TabContent } from "../components/Tabs";
import { buildClassName } from "../utils/common";

function Home() {
	const [tabIndex, setTabIndex] = useState(0);

	return (
		<div className="Home">
			<PageHeader>
				<PageHeaderTitle>Ad Marketplace</PageHeaderTitle>
			</PageHeader>

			<Tabs
				index={tabIndex}
				setIndex={setTabIndex}
				tabs={
					<>
						<div
							className={buildClassName("Tab", tabIndex === 0 && "active")}
							onClick={() => setTabIndex(0)}
						>
							<span>Influencers</span>
						</div>
						<div
							className={buildClassName("Tab", tabIndex === 1 && "active")}
							onClick={() => setTabIndex(1)}
						>
							<span>Campaigns</span>
						</div>
					</>
				}
			>
				<TabContent state={true}>
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
				</TabContent>
				<TabContent state={true}>
					<div className="Categories Section">
						<div className="title">Categories</div>
						<div className="Items">
							<Transition state eachElement eachElementDelay={20}>
								<div className="Item">
									<div className="icon">😂</div>
									<div className="body">
										<div className="title">Ads 1</div>
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
				</TabContent>
			</Tabs>
		</div>
	);
}

export default memo(Home);
