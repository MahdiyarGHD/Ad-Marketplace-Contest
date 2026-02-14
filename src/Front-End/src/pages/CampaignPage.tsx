import { memo, useEffect, useState } from "react";
import { useShallow } from "zustand/shallow";
import Avatar from "../components/Avatar";
import PageHeader, {
	PageHeaderButtons,
	PageHeaderTitle,
} from "../components/PageHeader";
import { Shimmer } from "../components/Shimmer";
import useCampaignStore, { type Campaign } from "../stores/useCampaignStore";
import { requestAPI } from "../utils/api";
import { backButton, popup } from "@tma.js/sdk-react";
import { invokeHapticFeedbackImpact } from "../utils/common";
import { useNavigate } from "react-router-dom";
import useCategoryStore from "../stores/useCategoryStore";
import {
	BanIcon,
	ChevronRight,
	MoreVerticalIcon,
	PencilIcon,
	TagIcon,
} from "lucide-react";
import {
	AdFormats,
	PriceTypes,
	type AdFormat,
	type PriceType,
} from "../stores/useChannelStore";
import useUIStore from "../stores/useUIStore";
import Menu, { DropdownMenu, MenuItem } from "../components/Menu";
import Transition from "../components/Transition";
import useAppStore from "../stores/useAppStore";
import Modal from "../components/Modal";
import Application from "../components/Application";
import useApplicationStore from "../stores/useApplicationStore";

function CampaignPage() {
	const [showApplication, setShowApplication] = useState<boolean>(false);

	const {
		id,
		advertiser_id,
		title,
		category_id,
		brief,
		description,
		budget_ton,
		max_price_per_placement,
		targeting,
		creative,
		status,
		starts_at,
		ends_at,
	} = useCampaignStore(useShallow((state) => state.activeCampaign)) || {};
	const { setActiveCampaign, setDraftCampaign } = useCampaignStore(
		useShallow((state) => ({
			setActiveCampaign: state.setActiveCampaign,
			setDraftCampaign: state.setDraftCampaign,
		})),
	);

	const setApplication = useApplicationStore(
		useShallow((state) => state.setApplication),
	);

	const clearApplication = useApplicationStore(
		useShallow((state) => state.clearApplication),
	);

	const userId = useAppStore(useShallow((state) => state.userId));

	const isOwn = advertiser_id === userId;

	const { getCategory } = useCategoryStore();

	const showToast = useUIStore(useShallow((state) => state.showToast));

	const category = getCategory(category_id ?? "");

	const apply = location.pathname.endsWith("/apply");

	const navigate = useNavigate();

	const getCampaignInfo = async () => {
		const response = await requestAPI(`/api/campaigns/${id}`, {}, "GET");

		setActiveCampaign(response.value);
	};

	const onBackButton = () => {
		window.history.back();
	};

	const onEdit = () => {
		setDraftCampaign({
			id,
			advertiser_id,
			title,
			category_id,
			category,
			brief,
			description,
			budget_ton,
			max_price_per_placement,
			targeting,
			creative,
			starts_at,
			ends_at,
		} as Campaign);

		navigate(`/edit-campaign/${id}`);
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

	const onApply = () => {
		if (isOwn) return;

		setApplication({
			campaign_id: id,
			campaign: {
				title,
				category_id,
				category,
				brief,
				description,
				budget_ton,
				max_price_per_placement,
				targeting,
				creative,
				starts_at,
				ends_at,
			} as Campaign,
			proposed_ad_format: targeting?.preferred_ad_formats?.[0],
			proposed_price_type: targeting?.preferred_price_types?.[0],
		});

		setShowApplication(true);
	};

	const onApplicationClose = () => {
		setShowApplication(false);

		clearApplication();
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
	}, [id]);

	useEffect(() => {
		if (status === 0) {
			useUIStore.setState({
				mainButton: { text: "Publish", onClick: handlePublish },
			});
		}
	}, [status]);

	useEffect(() => {
		if (advertiser_id && !isOwn) {
			useUIStore.setState({
				mainButton: { text: "Apply", onClick: onApply },
			});
		} else if (status !== 0) {
			useUIStore.setState({ mainButton: undefined });
		}
	}, [isOwn, advertiser_id]);

	useEffect(() => {
		console.log(apply);
		if (apply) {
			setShowApplication(true);
		}
	}, [apply]);

	return (
		<div className="scrollable">
			<div className="CampaignPage Profile">
				<PageHeader>
					<PageHeaderTitle>Campaign</PageHeaderTitle>
					<PageHeaderButtons>
						{isOwn && (
							<div className="Edit" onClick={onEdit}>
								<PencilIcon size={22} />
							</div>
						)}
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
								{budget_ton?.toFixed(2)} TON
							</Shimmer>
							<div className="subtitle">Budget</div>
						</div>
						<div className="Item">
							<Shimmer state={!!max_price_per_placement} className="title">
								{max_price_per_placement?.toFixed(2)} TON
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

				{isOwn && (
					<div className="Section">
						<div className="title">Manage</div>
						<div className="Items">
							<div
								className="Item"
								onClick={() => navigate(`/campaign/${id}/applications`)}
							>
								<div className="body">
									<div className="title">Applications</div>
								</div>
								<div className="meta">
									<ChevronRight />
								</div>
							</div>
							<div className="Item">
								<div className="body">
									<div className="title">Invitations</div>
								</div>
								<div className="meta">
									<ChevronRight />
								</div>
							</div>
						</div>
					</div>
				)}

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

				<div className="Section">
					<div className="Items">
						<div className="Item">
							<div className="body">
								<div className="title">Starts At</div>
							</div>
							<Shimmer className="meta" state={!!starts_at}>
								{new Date(starts_at ?? "").toLocaleString("en-US", {
									month: "long",
									day: "numeric",
									// hour12: false,
									// hour: "2-digit",
									// minute: "2-digit",
								}) || "N/A"}
							</Shimmer>
						</div>
						<div className="Item">
							<div className="body">
								<div className="title">Ends At</div>
							</div>
							<Shimmer className="meta" state={!!ends_at}>
								{new Date(ends_at ?? "").toLocaleString("en-US", {
									month: "long",
									day: "numeric",
									// hour12: false,
									// hour: "2-digit",
									// minute: "2-digit",
								}) || "N/A"}
							</Shimmer>
						</div>
					</div>
				</div>

				{!targeting && <LoadingSkeleton />}

				<Transition state={!!targeting} eachElement eachElementDelay={40}>
					{targeting && Object.keys(targeting).length > 0 && (
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
				<Modal
					open={showApplication}
					onClose={onApplicationClose}
					title="Application"
				>
					<Application type="campaign" onClose={onApplicationClose} />
				</Modal>
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
