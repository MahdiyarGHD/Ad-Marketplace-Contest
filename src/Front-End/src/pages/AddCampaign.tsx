import { memo, useEffect } from "react";
import { backButton, mainButton } from "@tma.js/sdk-react";
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
import { PriceTypes } from "../stores/useChannelStore";
import Menu, { DropdownMenu, MenuItem } from "../components/Menu";
import TextTransition from "../components/TextTransition";
import { requestAPI } from "../utils/api";
import useUIStore from "../stores/useUIStore";
import useCampaignStore, { type Campaign } from "../stores/useCampaignStore";

const AdFormats = ["Post"];

function AddCampaign() {
	const {
		draftCampaign,
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
			if (!draftCampaign.category_id) {
				throw new Error("Please select a category");
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
				targeting: {},
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
		mainButton.setText("Save");

		mainButton.show();

		backButton.show();

		backButton.onClick(onBackButton);

		invokeHapticFeedbackImpact("medium");

		return () => {
			mainButton.hide();

			backButton.hide();

			backButton.offClick(onBackButton);
		};
	}, []);

	useEffect(() => {
		mainButton.onClick(handleSave);

		return () => mainButton.offClick(handleSave);
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
							<input type="text" placeholder="Campaign Title" />
						</div>
					</div>
					<div className="Item">
						<div className="icon"></div>
						<div className="body">
							<textarea placeholder="Campaign Description" />
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
							<input type="text" placeholder="Budget" />
						</div>
						<div className="meta">TON</div>
					</div>
					<div className="Item">
						<div className="icon"></div>
						<div className="body">
							<input type="text" placeholder="Max Price Per Placement" />
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
								<input type="text" placeholder="Min" />
								<input type="text" placeholder="Max" />
							</div>
						</div>
					</div>
					<div className="Item">
						<div className="body">
							<div className="title">Min Average Views</div>
						</div>
						<div className="meta">
							<input type="text" placeholder="0" />
							<span>Views</span>
						</div>
					</div>
					<div className="Item">
						<div className="body">
							<div className="title">Min Premium Users</div>
						</div>
						<div className="meta">
							<input type="text" placeholder="0" />
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
														? PriceTypes[priceTypes[0]]
														: `${priceTypes.length} items`
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
							{PriceTypes.map((type, i) => (
								<MenuItem
									key={type}
									title={type}
									icon={
										draftCampaign.targeting?.preferred_price_types?.includes(i)
											? "✓"
											: ""
									}
									onClick={() => setDraftCampaignPriceType(i)}
								/>
							))}
						</DropdownMenu>
					</Menu>
				</div>
			</div>
		</div>
	);
}

export default memo(AddCampaign);
