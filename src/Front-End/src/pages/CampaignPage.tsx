import { memo, useEffect } from "react";
import { useShallow } from "zustand/shallow";
import Avatar from "../components/Avatar";
import PageHeader, {
	PageHeaderButtons,
	PageHeaderTitle,
} from "../components/PageHeader";
import { Shimmer } from "../components/Shimmer";
import useCampaignStore from "../stores/useCampaignStore";
import { requestAPI } from "../utils/api";
import { backButton, popup } from "@tma.js/sdk-react";
import { invokeHapticFeedbackImpact } from "../utils/common";
import { useNavigate } from "react-router-dom";
import useCategoryStore from "../stores/useCategoryStore";
import { BanIcon, MoreVerticalIcon, TagIcon } from "lucide-react";
import {
	AdFormats,
	PriceTypes,
	type AdFormat,
	type PriceType,
} from "../stores/useChannelStore";
import useUIStore from "../stores/useUIStore";
import Menu, { DropdownMenu, MenuItem } from "../components/Menu";
import Transition from "../components/Transition";

function CampaignPage() {
	const {
		id,
		title,
		category_id,
		brief,
		description,
		budget_ton,
		max_price_per_placement,
		targeting,
		creative,
		status,
		// starts_at,
		// ends_at,
	} = useCampaignStore(useShallow((state) => state.activeCampaign)) || {};
	const setActiveCampaign = useCampaignStore(
		useShallow((state) => state.setActiveCampaign),
	);

	const { getCategory } = useCategoryStore();

	const showToast = useUIStore(useShallow((state) => state.showToast));

	const category = getCategory(category_id ?? "");

	const navigate = useNavigate();

	const getCampaignInfo = async () => {
		const response = await requestAPI(`/api/campaigns/${id}`, {}, "GET");

		setActiveCampaign(response.value);
	};

	const onBackButton = () => {
		window.history.back();
	};

	const handlePublish = async () => {
		const response = await requestAPI(
			`/api/campaigns/${id}/status`,
			{ status: 1 },
			"PATCH",
		);

		console.log(response);

		if (!response?.is_error) {
			useUIStore.setState({ mainButton: { text: "Published" } });
			getCampaignInfo();
		} else {
			showToast({ title: response.first_error?.description });
		}
	};

	useEffect(() => {
		backButton.show();

		backButton.onClick(onBackButton);

		invokeHapticFeedbackImpact("medium");

		return () => {
			backButton.hide();

			backButton.offClick(onBackButton);
		};
	}, []);

	useEffect(() => {
		if (!id) return;

		getCampaignInfo();

		if (status === 0) {
			useUIStore.setState({
				mainButton: { text: "Publish", onClick: handlePublish },
			});
		}
	}, [id, status]);

	return (
		<div className="scrollable">
			<div className="CampaignPage Profile">
				<PageHeader>
					<PageHeaderTitle>Campaign</PageHeaderTitle>
					<PageHeaderButtons>
						<div className="More">
							<Menu
								custom={({ onClick }) => (
									<MoreVerticalIcon
										onClick={() => {
											onClick();
											invokeHapticFeedbackImpact("light");
										}}
									/>
								)}
							>
								<DropdownMenu className="right">
									<MenuItem
										title="Cancel Campaign"
										icon={<BanIcon />}
										className="destructive"
										onClick={() => {
											popup.show({
												title: "Cancel Campaign",
												message:
													"Are you sure you want to cancel this campaign?",
												buttons: [
													{
														text: "Cancel Campaign",
														type: "destructive",
													},
												],
											});
										}}
									/>
								</DropdownMenu>
							</Menu>
						</div>
					</PageHeaderButtons>
				</PageHeader>

				<div className="Campaign">
					<Avatar id={id ?? ""} size={80} isCampaign />
					<div className="info">
						<Shimmer className="title">{title}</Shimmer>
						<div className="subtitle">{brief ?? "campaign"}</div>
					</div>
				</div>

				<div className="Section">
					<div className="title">Pricing</div>
					<div className="Statistics">
						<div className="Item">
							<Shimmer state={!!budget_ton} className="title">
								{budget_ton} TON
							</Shimmer>
							<div className="subtitle">Budget</div>
						</div>
						<div className="Item">
							<Shimmer state={!!max_price_per_placement} className="title">
								{max_price_per_placement} TON
							</Shimmer>
							<div className="subtitle">Max Price Per Placement</div>
						</div>
					</div>
				</div>

				<div className="Items">
					<div
						className="Item"
						onClick={() => navigate(`/category/${category_id}`)}
					>
						<div className="icon">{category?.icon ?? <TagIcon />}</div>
						<div className="body">
							<Shimmer className="title">{category?.name}</Shimmer>
						</div>
					</div>
				</div>

				<div className="Section">
					<div className="title">Description</div>
					<div className="Items">
						<div className="Item">
							<div className="body">
								<Shimmer className="title preWrap">{description}</Shimmer>
							</div>
						</div>
					</div>
				</div>

				{!targeting && <LoadingSkeleton />}

				<Transition state={!!targeting} eachElement eachElementDelay={40}>
					{targeting && (
						<div className="Section">
							<div className="title">Targeting</div>
							<div className="Items">
								<Transition state eachElement eachElementDelay={40}>
									{targeting?.min_subscribers > 0 && (
										<div className="Item">
											<div className="body">
												<div className="title">Min Subscribers</div>
											</div>
											<div className="meta">{targeting?.min_subscribers}</div>
										</div>
									)}
									{targeting?.max_subscribers > 0 && (
										<div className="Item">
											<div className="body">
												<div className="title">Max Subscribers</div>
											</div>
											<div className="meta">{targeting?.max_subscribers}</div>
										</div>
									)}
									{targeting?.min_average_views > 0 && (
										<div className="Item">
											<div className="body">
												<div className="title">Min Average Views</div>
											</div>
											<div className="meta">{targeting?.min_average_views}</div>
										</div>
									)}
									{targeting?.min_premium_count > 0 && (
										<div className="Item">
											<div className="body">
												<div className="title">Min Premium Users</div>
											</div>
											<div className="meta">{targeting?.min_premium_count}</div>
										</div>
									)}
								</Transition>
							</div>
						</div>
					)}
					{!!targeting?.preferred_category_ids?.length && (
						<div className="Section">
							<div className="title">Preferred Categories</div>
							<div className="Items">
								<Transition state eachElement eachElementDelay={40}>
									{targeting?.preferred_category_ids.map((cat_id) => {
										const category = getCategory(cat_id);
										return (
											<div
												className="Item"
												key={cat_id}
												onClick={() => navigate(`/category/${cat_id}`)}
											>
												<div className="icon">
													{category?.icon ?? <TagIcon />}
												</div>
												<div className="body">
													<div className="title">{category?.name}</div>
												</div>
											</div>
										);
									})}
								</Transition>
							</div>
						</div>
					)}
					{!!targeting?.preferred_ad_formats?.length && (
						<div className="Section">
							<div className="title">Preferred Ad Formats</div>
							<div className="Items">
								<Transition state eachElement eachElementDelay={40}>
									{targeting?.preferred_ad_formats.map((item) => (
										<div className="Item" key={item}>
											<div className="body">
												<div className="title">
													{AdFormats[item as AdFormat] || "Post"}
												</div>
											</div>
										</div>
									))}
								</Transition>
							</div>
						</div>
					)}
					{!!targeting?.preferred_price_types?.length && (
						<div className="Section">
							<div className="title">Preferred Price Types</div>
							<div className="Items">
								<Transition state eachElement eachElementDelay={40}>
									{targeting?.preferred_price_types.map((item) => (
										<div className="Item" key={item}>
											<div className="body">
												<div className="title">
													{PriceTypes[item as PriceType] || "Per hour"}
												</div>
											</div>
										</div>
									))}
								</Transition>
							</div>
						</div>
					)}
					{!!creative?.call_to_action_buttons?.length && (
						<div className="Section">
							<div className="title">Call To Action Buttons</div>
							<div className="Items">
								<Transition state eachElement eachElementDelay={40}>
									{creative.call_to_action_buttons.map((button) => (
										<div className="Item" key={button.text}>
											<div className="body">
												<div className="title">{button.text}</div>
												<div className="meta">{button.url}</div>
											</div>
										</div>
									))}
								</Transition>
							</div>
						</div>
					)}
				</Transition>
			</div>
		</div>
	);
}

const LoadingSkeleton = memo(() => (
	<div className="Skeleton">
		<div className="Section">
			<div className="title Shimmer"></div>
			<div className="Items">
				{Array.from({ length: 2 }).map(() => (
					<div className="Item" key={Math.random()}>
						<div className="body">
							<div className="title Shimmer"></div>
						</div>
						<div className="meta Shimmer" style={{ width: "4rem" }}></div>
					</div>
				))}
			</div>
		</div>
	</div>
));

export default memo(CampaignPage);
