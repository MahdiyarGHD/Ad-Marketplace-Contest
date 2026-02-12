import { memo, useEffect, useState } from "react";
import PageHeader, {
	PageHeaderButtons,
	PageHeaderTitle,
} from "../components/PageHeader";
import {
	ChevronRight,
	MegaphoneIcon,
	MessageCircleIcon,
	PlusIcon,
	SearchIcon,
} from "lucide-react";
import "./Home.scss";
import Transition from "../components/Transition";
import Tabs, { TabContent } from "../components/Tabs";
import { buildClassName, invokeHapticFeedbackImpact } from "../utils/common";
import useCategoryStore, { type Category } from "../stores/useCategoryStore";
import useChannelStore, { type Channel } from "../stores/useChannelStore";
import Avatar from "../components/Avatar";
import { useNavigate } from "react-router-dom";
import useAppStore from "../stores/useAppStore";
import Menu, { DropdownMenu, MenuItem } from "../components/Menu";
import RLottie from "../components/RLottie";

const renderCategory = (
	category: Category,
	onClick: (categoryId: string) => void,
) => (
	<div className="Item" key={category.id} onClick={() => onClick(category.id)}>
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

export const renderChannel = (
	channel: Channel,
	onClick: (channel: Channel) => void,
) => (
	<div className="ChatItem" key={channel.id} onClick={() => onClick(channel)}>
		<Avatar id={channel.chat_id} title={channel.title} photo="" />
		<div className="body">
			<div className="title">{channel.title}</div>
			<div className="subtitle">{channel.subscriber_count} subscribers</div>
		</div>
		<div className="meta">
			{/* <div className="count">12</div> */}
			<ChevronRight />
		</div>
	</div>
);

function Home() {
	const [tabIndex, setTabIndex] = useState(0);

	const { isAuth } = useAppStore();

	const { getCategories, getCategoryByName } = useCategoryStore();
	const {
		influencers,
		getInfluencers,
		campaigns,
		getCampaigns,
		setActiveChannel,
	} = useChannelStore();

	const navigate = useNavigate();

	const showChannelProfile = (channel: Channel) => {
		setActiveChannel(channel);

		navigate(`/channel/${channel.id}`);
	};

	const showCategoryPageByName = (categoryName: string) => {
		const category = getCategoryByName(categoryName);
		if (category) {
			navigate(`/category/${category.id}`);
		}
	};
	const showCategoryPageById = (categoryId: string) => {
		navigate(`/category/${categoryId}`);
	};

	useEffect(() => {
		if (!isAuth) return;

		getCategories();
		getInfluencers();
		getCampaigns();
	}, [isAuth]);

	const renderSection = (element: {
		$type: string;
		icon: string;
		label: string;
		items: Channel[] | Category[];
	}) => {
		return (
			<div className="Section" key={element.label}>
				<div
					className="flex pointer"
					onClick={() => {
						element.$type === "channel" &&
							showCategoryPageByName(element.label);
						element.$type === "category" && navigate("/categories");
					}}
				>
					{/* <div className="icon">{element.icon}</div> */}
					<h2 className="title">{element.label}</h2>
					<div className="meta">
						Show All <ChevronRight size={18} />
					</div>
				</div>
				<div
					className={buildClassName(
						"Items",
						element.label === "Top Picks" && "row scrollable x",
					)}
				>
					<Transition state eachElement eachElementDelay={40}>
						{element.items.map((item) =>
							element.$type === "channel"
								? renderChannel(item as Channel, showChannelProfile)
								: renderCategory(item as Category, showCategoryPageById),
						)}
					</Transition>
				</div>
			</div>
		);
	};

	return (
		<div className="Home">
			<PageHeader>
				<PageHeaderTitle>Ad Marketplace</PageHeaderTitle>
				<PageHeaderButtons>
					<div className="Add">
						<Menu
							custom={({ onClick }) => (
								<PlusIcon
									onClick={() => {
										onClick();
										invokeHapticFeedbackImpact("light");
									}}
								/>
							)}
						>
							<DropdownMenu className="right">
								<MenuItem
									title="Add Channel"
									icon={<MessageCircleIcon />}
									onClick={() => navigate("/add-channel")}
								/>
								<MenuItem
									title="Add Campaign"
									icon={<MegaphoneIcon />}
									onClick={() => navigate("/add-campaign")}
								/>
							</DropdownMenu>
						</Menu>
					</div>
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
					{influencers.elements.map((element) => renderSection(element))}
					{influencers.elements.length === 0 && <LoadingSkeleton />}
				</TabContent>
				<TabContent state={true}>
					{campaigns.elements.map((element) => renderSection(element))}
					{campaigns.elements.length === 0 && <NoCampaignPlaceholder />}
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

const NoCampaignPlaceholder = memo(() => (
	<div className="Placeholder">
		<div className="Emoji">
			<RLottie sticker="pepe" autoplay width={120} height={120} />
		</div>
		<h2 className="Title">No Campaigns Yet</h2>
		<p className="Subtitle">There is no campaign yet. Gerye kon</p>
	</div>
));

export default memo(Home);
