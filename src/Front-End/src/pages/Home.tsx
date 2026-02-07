import { memo, useEffect, useState } from "react";
import PageHeader, {
	PageHeaderButtons,
	PageHeaderTitle,
} from "../components/PageHeader";
import { ChevronRight, SearchIcon } from "lucide-react";
import "./Home.scss";
import Transition from "../components/Transition";
import Tabs, { TabContent } from "../components/Tabs";
import { buildClassName } from "../utils/common";
import useCategoryStore, { type Category } from "../stores/useCategoryStore";
import useChannelStore, { type Channel } from "../stores/useChannelStore";
import Avatar from "../components/Avatar";
import { useNavigate } from "react-router-dom";
import useAppStore from "../stores/useAppStore";

function Home() {
	const [tabIndex, setTabIndex] = useState(0);

	const { isAuth } = useAppStore();

	const { getCategories } = useCategoryStore();
	const { influencers, getInfluencers, setActiveChannel } = useChannelStore();

	const navigate = useNavigate();

	const showChannelProfile = (channel: Channel) => {
		setActiveChannel(channel);

		navigate(`/channel/${channel.id}`);
	};

	useEffect(() => {
		if (!isAuth) return;

		getCategories();
		getInfluencers();
	}, [isAuth]);

	const renderCategory = (category: Category) => (
		<div className="Item" key={category.id}>
			<div className="icon">{category.icon}</div>
			<div className="body">
				<div className="title">{category.name}</div>
				{/* <div className="subtitle">Description</div> */}
			</div>
			<div className="meta">
				{/* <div className="count">12</div> */}
				<ChevronRight />
			</div>
		</div>
	);

	const renderChannel = (channel: Channel) => (
		<div
			className="ChatItem"
			key={channel.id}
			onClick={() => showChannelProfile(channel)}
		>
			<Avatar id={channel.chat_id} title={channel.title} photo="" />
			<div className="body">
				<div className="title">{channel.title}</div>
				{/* <div className="subtitle">Description</div> */}
			</div>
			<div className="meta">
				{/* <div className="count">12</div> */}
				<ChevronRight />
			</div>
		</div>
	);

	return (
		<div className="Home">
			<PageHeader>
				<PageHeaderTitle>Ad Marketplace</PageHeaderTitle>
				<PageHeaderButtons>
					<div className="Search">
						<SearchIcon />
					</div>
				</PageHeaderButtons>
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
				<TabContent state={true} className="scrollable">
					{influencers.elements.map((element) => (
						<div className="Section" key={element.label}>
							<div className="flex">
								<div className="icon">{element.icon}</div>
								<h2 className="title">{element.label}</h2>
								<div className="meta">
									Show All <ChevronRight size={18} />
								</div>
							</div>
							<div className="Items">
								<Transition state eachElement eachElementDelay={20}>
									{element.items.map((item) =>
										element.$type === "channel"
											? renderChannel(item as Channel)
											: renderCategory(item as Category),
									)}
								</Transition>
							</div>
						</div>
					))}
					{influencers.elements.length === 0 && <LoadingSkeleton />}
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

const LoadingSkeleton = memo(() => (
	<div className="Skeleton">
		{Array.from({ length: 3 }).map(() => (
			<div className="Section" key={Math.random()}>
				<div className="flex">
					<div className="title Shimmer"></div>
				</div>
				<div className="Items">
					{Array.from({ length: 5 }).map(() => (
						<div className="Item" key={Math.random()}>
							<div className="Avatar" />
							<div className="body">
								<div className="title Shimmer" />
								<div className="subtitle Shimmer" />
							</div>
						</div>
					))}
				</div>
			</div>
		))}
	</div>
));

export default memo(Home);
