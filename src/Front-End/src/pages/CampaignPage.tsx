import { memo, useEffect } from "react";
import { useShallow } from "zustand/shallow";
import Avatar from "../components/Avatar";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";
import { Shimmer } from "../components/Shimmer";
import useCampaignStore from "../stores/useCampaignStore";
import { requestAPI } from "../utils/api";
import { backButton } from "@tma.js/sdk-react";
import { invokeHapticFeedbackImpact } from "../utils/common";
import { useNavigate } from "react-router-dom";
import useCategoryStore from "../stores/useCategoryStore";
import { TagIcon } from "lucide-react";
import {
	AdFormats,
	PriceTypes,
	type AdFormat,
	type PriceType,
} from "../stores/useChannelStore";

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
		// starts_at,
		// ends_at,
	} = useCampaignStore(useShallow((state) => state.activeCampaign)) || {};
	const { setActiveCampaign } = useCampaignStore();

	const { getCategory } = useCategoryStore();

	const category = getCategory(category_id ?? "");

	const navigate = useNavigate();

	const getCampaignInfo = async () => {
		const response = await requestAPI(`/api/campaigns/${id}`, {}, "GET");

		setActiveCampaign(response.value);
		console.log(response);
	};

	const onBackButton = () => {
		window.history.back();
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

	return (
		<div className="scrollable">
			<div className="CampaignPage Profile">
				<PageHeader>
					<PageHeaderTitle>Campaign</PageHeaderTitle>
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
							<div className="title">{budget_ton} TON</div>
							<div className="subtitle">Budget</div>
						</div>
						<div className="Item">
							<div className="title">{max_price_per_placement} TON</div>
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

				{targeting && (
					<div className="Section">
						<div className="title">Targeting</div>
						<div className="Items">
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
						</div>
					</div>
				)}
				{!!targeting?.preferred_category_ids?.length && (
					<div className="Section">
						<div className="title">Preferred Categories</div>
						<div className="Items">
							{targeting?.preferred_category_ids.map((cat_id) => {
								const category = getCategory(cat_id);
								return (
									<div
										className="Item"
										key={cat_id}
										onClick={() => navigate(`/category/${cat_id}`)}
									>
										<div className="icon">{category?.icon ?? <TagIcon />}</div>
										<div className="body">
											<div className="title">{category?.name}</div>
										</div>
									</div>
								);
							})}
						</div>
					</div>
				)}
				{!!targeting?.preferred_ad_formats?.length && (
					<div className="Section">
						<div className="title">Preferred Ad Formats</div>
						<div className="Items">
							{targeting?.preferred_ad_formats.map((item) => (
								<div className="Item" key={item}>
									<div className="body">
										<div className="title">
											{AdFormats[item as AdFormat] || "Post"}
										</div>
									</div>
								</div>
							))}
						</div>
					</div>
				)}
				{!!targeting?.preferred_price_types?.length && (
					<div className="Section">
						<div className="title">Preferred Price Types</div>
						<div className="Items">
							{targeting?.preferred_price_types.map((item) => (
								<div className="Item" key={item}>
									<div className="body">
										<div className="title">
											{PriceTypes[item as PriceType] || "Per hour"}
										</div>
									</div>
								</div>
							))}
						</div>
					</div>
				)}
				{!!creative?.call_to_action_buttons?.length && (
					<div className="Section">
						<div className="title">Call To Action Buttons</div>
						<div className="Items">
							{creative.call_to_action_buttons.map((button) => (
								<div className="Item" key={button.text}>
									<div className="body">
										<div className="title">{button.text}</div>
										<div className="meta">{button.url}</div>
									</div>
								</div>
							))}
						</div>
					</div>
				)}
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
