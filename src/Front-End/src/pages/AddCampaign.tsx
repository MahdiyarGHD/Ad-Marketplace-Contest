import { memo, useEffect, useState } from "react";
import { backButton } from "@tma.js/sdk-react";
import { invokeHapticFeedbackImpact } from "../utils/common";
import { useNavigate } from "react-router";
import "./SetChannelData.scss";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";
import {
	ChevronDown,
	ChevronRight,
	DollarSignIcon,
	MegaphoneIcon,
	TagIcon,
} from "lucide-react";
import { PriceTypes, type PriceType } from "../stores/useChannelStore";
import Menu, { DropdownMenu, MenuItem } from "../components/Menu";
import TextTransition from "../components/TextTransition";
import { requestAPI } from "../utils/api";
import useUIStore from "../stores/useUIStore";
import useCampaignStore, { type Campaign } from "../stores/useCampaignStore";
import DatePicker from "../components/DatePicker";

const AdFormats = ["Post"];

function AddCampaign() {
	const [showDatePicker, setShowDatePicker] = useState(false);

	const {
		draftCampaign,
		setDraftCampaign,
		clearDraftCampaign,
		setDraftCampaignAdFormat,
		setDraftCampaignPriceType,
	} = useCampaignStore();

	const { showToast } = useUIStore();

	const navigate = useNavigate();

	const onBackButton = () => {
		window.history.back();
	};

	const onSelectCategory = () => {
		navigate("/select-category/campaign");
	};

	const onSelectPreferredCategory = () => {
		navigate("/select-category/campaign-preferred");
	};

	const handleSave = async () => {
		try {
			if (!draftCampaign.title) {
				throw new Error("Please enter a campaign title");
			}
			if (!draftCampaign.description) {
				throw new Error("Please enter a campaign description");
			}
			if (!draftCampaign.category_id) {
				throw new Error("Please select a category");
			}
			if (!draftCampaign.budget_ton) {
				throw new Error("Please enter a budget");
			}
			if (
				draftCampaign.targeting.min_subscribers >
				draftCampaign.targeting.max_subscribers
			) {
				throw new Error(
					"Min subscribers cannot be greater than max subscribers",
				);
			}
		} catch (error) {
			showToast({ title: (error as Error).message });
			return;
		}

		const response = await requestAPI(
			"/api/campaigns/",
			{
				title: draftCampaign.title,
				category_id: draftCampaign.category_id,
				targeting: draftCampaign.targeting,
				budget_ton: draftCampaign.budget_ton,
				max_price_per_placement: draftCampaign.max_price_per_placement,
				description: draftCampaign.description,
				starts_at: draftCampaign.starts_at,
				ends_at: draftCampaign.ends_at,
				creative: draftCampaign.creative,
				brief: draftCampaign.brief,
			} as Partial<Campaign>,
			"POST",
		);

		if (!response.isError && response.value) {
			navigate("/add-campaign/success");
			invokeHapticFeedbackImpact("medium");
			setTimeout(() => invokeHapticFeedbackImpact("soft"), 200);
			setTimeout(() => invokeHapticFeedbackImpact("soft"), 300);
			clearDraftCampaign();
		} else {
			showToast({ title: response.first_error.description });
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
		useUIStore.setState({
			mainButton: {
				text: "Save",
				onClick: handleSave,
			},
		});
	}, [draftCampaign]);

	return (
		<div className="SetChannelData scrollable">
			<PageHeader>
				<PageHeaderTitle>Create your campaign</PageHeaderTitle>
			</PageHeader>

			<div className="Section">
				<div className="Items">
					<div className="Item">
						<div className="icon">
							<MegaphoneIcon />
						</div>
						<div className="body">
							<input
								type="text"
								placeholder="Campaign Title"
								value={draftCampaign.title}
								onChange={(e) => setDraftCampaign({ title: e.target.value })}
							/>
						</div>
					</div>
					<div className="Item">
						<div className="icon"></div>
						<div className="body">
							<textarea
								placeholder="Campaign Description"
								value={draftCampaign.description}
								onChange={(e) =>
									setDraftCampaign({ description: e.target.value })
								}
							/>
						</div>
					</div>
					<div className="Item" onClick={onSelectCategory}>
						<div className="icon">
							<TagIcon />
						</div>
						<div className="body">
							<div className="title">
								{draftCampaign?.category?.name || "Select Category..."}
							</div>
						</div>
						<div className="meta">
							<ChevronRight />
						</div>
					</div>
					<div className="Item">
						<div className="icon">
							<DollarSignIcon />
						</div>
						<div className="body">
							<input
								type="number"
								placeholder="Budget"
								value={draftCampaign.budget_ton || ""}
								onChange={(e) =>
									setDraftCampaign({ budget_ton: Number(e.target.value) })
								}
							/>
						</div>
						<div className="meta">TON</div>
					</div>
					<div className="Item">
						<div className="icon"></div>
						<div className="body">
							<input
								type="number"
								placeholder="Max Price Per Placement"
								value={draftCampaign.max_price_per_placement || ""}
								onChange={(e) =>
									setDraftCampaign({
										max_price_per_placement: Number(e.target.value),
									})
								}
							/>
						</div>
						<div className="meta">TON</div>
					</div>
				</div>
			</div>
			<div className="Section Targeting">
				<div className="title">Targeting</div>
				<div className="Items">
					<div className="Item">
						<div className="body multiline">
							<div className="title">Subscribers Count</div>
							<div className="flex">
								<input
									type="number"
									placeholder="Min"
									value={draftCampaign.targeting?.min_subscribers || ""}
									onChange={(e) =>
										setDraftCampaign({
											targeting: {
												...draftCampaign.targeting,
												min_subscribers: Number(e.target.value),
											},
										})
									}
								/>
								<input
									type="number"
									placeholder="Max"
									value={draftCampaign.targeting?.max_subscribers || ""}
									onChange={(e) =>
										setDraftCampaign({
											targeting: {
												...draftCampaign.targeting,
												max_subscribers: Number(e.target.value),
											},
										})
									}
								/>
							</div>
						</div>
					</div>
					<div className="Item">
						<div className="body">
							<div className="title">Min Average Views</div>
						</div>
						<div className="meta">
							<input
								type="number"
								placeholder="0"
								value={draftCampaign.targeting?.min_average_views || ""}
								onChange={(e) =>
									setDraftCampaign({
										targeting: {
											...draftCampaign.targeting,
											min_average_views: Number(e.target.value),
										},
									})
								}
							/>
							<span>Views</span>
						</div>
					</div>
					<div className="Item">
						<div className="body">
							<div className="title">Min Premium Users</div>
						</div>
						<div className="meta">
							<input
								type="number"
								placeholder="0"
								value={draftCampaign.targeting?.min_premium_count || ""}
								onChange={(e) =>
									setDraftCampaign({
										targeting: {
											...draftCampaign.targeting,
											min_premium_count: Number(e.target.value),
										},
									})
								}
							/>
							<span>Users</span>
						</div>
					</div>
					<div className="Item" onClick={onSelectPreferredCategory}>
						<div className="body">
							<div className="title">Preferred Categories</div>
						</div>
						<div className="meta">
							{draftCampaign?.targeting?.preferred_category_ids?.length
								? `${draftCampaign.targeting.preferred_category_ids.length} Selected`
								: "Select"}
							<ChevronRight />
						</div>
					</div>
					<Menu
						closeManually
						custom={({ onClick }) => {
							const adFormats =
								draftCampaign.targeting?.preferred_ad_formats || [];
							return (
								<div className="Item" onClick={onClick}>
									<div className="body">Preferred Ad Formats</div>
									<div className="meta">
										<TextTransition
											text={
												adFormats.length > 0
													? adFormats.length === 1
														? AdFormats[adFormats[0]]
														: `${adFormats.length} items`
													: "None"
											}
										/>
										<ChevronDown />
									</div>
								</div>
							);
						}}
					>
						<DropdownMenu className="right">
							{AdFormats.map((type, i) => (
								<MenuItem
									key={type}
									title={type}
									icon={
										draftCampaign.targeting?.preferred_ad_formats?.includes(i)
											? "✓"
											: ""
									}
									onClick={() => setDraftCampaignAdFormat(i)}
								/>
							))}
						</DropdownMenu>
					</Menu>
					<Menu
						closeManually
						custom={({ onClick }) => {
							const priceTypes =
								draftCampaign.targeting?.preferred_price_types || [];
							return (
								<div className="Item" onClick={onClick}>
									<div className="body">Preferred Price Types</div>
									<div className="meta">
										<TextTransition
											text={
												priceTypes.length > 0
													? priceTypes.length === 1
														? PriceTypes[priceTypes[0] as PriceType]
														: `${priceTypes.length} Selected`
													: "None"
											}
										/>
										<ChevronDown />
									</div>
								</div>
							);
						}}
					>
						<DropdownMenu className="right">
							{Object.entries(PriceTypes).map(([key, type]) => (
								<MenuItem
									key={key}
									title={type}
									icon={
										draftCampaign.targeting?.preferred_price_types?.includes(
											Number(key),
										)
											? "✓"
											: ""
									}
									onClick={() => setDraftCampaignPriceType(Number(key))}
								/>
							))}
						</DropdownMenu>
					</Menu>
				</div>
				<div className="Section">
					<div className="Items">
						<div className="Item" onClick={() => setShowDatePicker(true)}>
							<div className="body">
								<div className="title">Select Date</div>
							</div>
							<div className="meta">
								{draftCampaign.starts_at &&
									new Date(draftCampaign.starts_at).toLocaleDateString() +
										" - " +
										new Date(draftCampaign.ends_at).toLocaleDateString()}
								<ChevronRight />
							</div>
						</div>
						<DatePicker
							show={showDatePicker}
							title="Select Date"
							selected={
								draftCampaign.starts_at
									? {
											from: new Date(draftCampaign.starts_at),
											to: new Date(draftCampaign.ends_at),
										}
									: undefined
							}
							onSelect={(date) =>
								setDraftCampaign({
									...draftCampaign,
									starts_at: date?.from?.toISOString(),
									ends_at: date?.to?.toISOString(),
								})
							}
							onClose={() => setShowDatePicker(false)}
						/>
					</div>
				</div>
			</div>
		</div>
	);
}

export default memo(AddCampaign);
