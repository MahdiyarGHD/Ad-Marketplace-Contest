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
import type { Campaign } from "../stores/useCampaignStore";
import useCampaignStore from "../stores/useCampaignStore";
import { useShallow } from "zustand/shallow";
import Search from "../components/Search";
import { requestAPI } from "../utils/api";
import type { FilterValues } from "../stores/useUIStore";
import useUIStore from "../stores/useUIStore";
import { retrieveLaunchParams } from "@tma.js/sdk-react";

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
			<div className="title" dir="auto">
				{channel.title}
			</div>
			<div className="subtitle">{channel.subscriber_count} subscribers</div>
		</div>
		<div className="meta">
			{/* <div className="count">12</div> */}
			<ChevronRight />
		</div>
	</div>
);

export const renderCampaign = (
	campaign: Campaign,
	onClick: (campaign: Campaign) => void,
) => (
	<div className="ChatItem" key={campaign.id} onClick={() => onClick(campaign)}>
		<Avatar id={campaign.id ?? ""} title={campaign.title} isCampaign />
		<div className="body">
			<div className="title" dir="auto">
				{campaign.title}
			</div>
			<div className="subtitle" dir="auto">
				{campaign.brief || campaign.description}
			</div>
		</div>
		<div className="meta">
			<ChevronRight />
		</div>
	</div>
);

function Home() {
	const [tabIndex, setTabIndex] = useState(0);

	const { isAuth, startParamHandled } = useAppStore();

	const { getCategories } = useCategoryStore();
	const {
		influencers,
		getInfluencers,
		campaigns,
		getCampaigns,
		setActiveChannel,
	} = useChannelStore();
	const setActiveCampaign = useCampaignStore(
		useShallow((state) => state.setActiveCampaign),
	);

	const channelSearchResults = useUIStore(
		useShallow((state) => state.search.channels.results),
	);
	const campaignSearchResults = useUIStore(
		useShallow((state) => state.search.campaigns.results),
	);

	const setChannelSearchResults = useUIStore(
		useShallow((state) => state.search.channels.setResults),
	);
	const setCampaignSearchResults = useUIStore(
		useShallow((state) => state.search.campaigns.setResults),
	);

	const launchParams = retrieveLaunchParams();

	const navigate = useNavigate();

	const handleStartParam = async () => {
		if (launchParams.tgWebAppStartParam) {
			const [type, id] = launchParams.tgWebAppStartParam.split("_", 2);

			launchParams.tgWebAppStartParam = "";

			switch (type) {
				case "deal":
					navigate(`/deals/${id}`);
					break;

				case "application":
					navigate(`/application/${id}`);
					break;

				case "invitation":
					navigate(`/invitation/${id}`);
					break;
			}

			useAppStore.setState({ startParamHandled: true });
		}
	};

	const showChannelProfile = (channel: Channel) => {
		setActiveChannel(channel);

		navigate(`/channel/${channel.id}`);
	};

	const showCampaignPage = (campaign: Campaign) => {
		setActiveCampaign(campaign);

		navigate(`/campaign/${campaign.id}`);
	};

	// const showCategoryPageByName = (categoryName: string) => {
	// 	const category = getCategoryByName(categoryName);
	// 	if (category) {
	// 		navigate(`/category/${category.id}`);
	// 	}
	// };

	const showCategoryPageById = (categoryId: string, type: string) => {
		navigate(`/category/${categoryId}?type=${type}`);
	};

	const onSearch = async (
		query?: string,
		filters?: FilterValues,
		type?: "channels" | "campaigns",
	) => {
		if (query === "" && (!filters || Object.keys(filters).length === 0)) {
			if (type === "channels") {
				setChannelSearchResults?.(undefined);
			} else if (type === "campaigns") {
				setCampaignSearchResults?.(undefined);
			}
			return;
		}

		const response = await requestAPI(
			`/api/${type}/search`,
			{
				query: query || "",
				...filters,
			},
			"GET",
		);

		if (type === "channels") {
			setChannelSearchResults?.(response.value[type!] as Channel[]);
		} else if (type === "campaigns") {
			setCampaignSearchResults?.(response.value[type!] as Campaign[]);
		}
		console.log("Search", response);
	};

	useEffect(() => {
		if (!isAuth) return;

		if (!startParamHandled) handleStartParam();

		getCategories();
		getInfluencers();
		getCampaigns();
	}, [isAuth]);

	const renderSection = (
		element: {
			$type: string;
			icon: string;
			label: string;
			items: Channel[] | Category[] | Campaign[];
		},
		type: "channel" | "campaign",
	) => {
		return (
			<div className="Section" key={element.label}>
				<div
					className="flex pointer"
					onClick={() => {
						(element.$type === "channel" || element.$type === "campaign") &&
							showCategoryPageById(
								(element.items[0] as Channel | Campaign).category?.id || "",
								type,
							);
						element.$type === "category" && navigate(`/categories/${type}`);
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
								: element.$type === "campaign"
									? renderCampaign(item as Campaign, showCampaignPage)
									: renderCategory(item as Category, (categoryId) =>
											showCategoryPageById(categoryId, type),
										),
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
				<TabContent state={true} className="">
					<Search type="channels" onSearch={onSearch} />
					{channelSearchResults && channelSearchResults !== undefined ? (
						channelSearchResults.length > 0 ? (
							renderSection(
								{
									$type: "channel",
									icon: "",
									label: "Search Results",
									items: channelSearchResults,
								},
								"channel",
							)
						) : (
							<NoResultsPlaceholder />
						)
					) : (
						influencers.elements.map((element) =>
							renderSection(element, "channel"),
						)
					)}
					{influencers.elements.length === 0 && <LoadingSkeleton />}
				</TabContent>
				<TabContent state={true} className="">
					<Search type="campaigns" onSearch={onSearch} />
					{campaignSearchResults && campaignSearchResults !== undefined ? (
						campaignSearchResults.length > 0 ? (
							renderSection(
								{
									$type: "campaign",
									icon: "",
									label: "Search Results",
									items: campaignSearchResults,
								},
								"campaign",
							)
						) : (
							<NoResultsPlaceholder />
						)
					) : (
						campaigns.elements.map((element) =>
							renderSection(element, "campaign"),
						)
					)}
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
			<RLottie sticker="notfound" autoplay width={120} height={120} />
		</div>
		<h2 className="Title">No Campaigns Yet</h2>
		<p className="Subtitle">There is no campaign yet.</p>
	</div>
));

const NoResultsPlaceholder = memo(() => (
	<div className="Placeholder">
		<div className="Emoji">
			<RLottie sticker="notfound" autoplay width={120} height={120} />
		</div>
		<h2 className="Title">No Results</h2>
		<p className="Subtitle">There are no results for your search.</p>
	</div>
));

export default memo(Home);
