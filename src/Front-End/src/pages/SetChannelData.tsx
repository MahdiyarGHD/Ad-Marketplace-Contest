import { memo, useEffect } from "react";
import RLottie from "../components/RLottie";
import { backButton, mainButton } from "@tma.js/sdk-react";
import { invokeHapticFeedbackImpact } from "../utils/common";
import { useNavigate } from "react-router";
import "./SetChannelData.scss";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";
import Avatar from "../components/Avatar";
import {
	ChevronDown,
	ChevronRight,
	ClockIcon,
	DollarSignIcon,
	PlusIcon,
	SendHorizontalIcon,
	TagIcon,
} from "lucide-react";
import useChannelStore from "../stores/useChannelStore";
import Menu, { DropdownMenu, MenuItem } from "../components/Menu";

const PriceTypes = ["Per hour", "Per day", "Per 1000 views"];
const AdFormats = ["Post"];

function SetChannelData() {
	const { draftChannel, setDraftChannelPriceType, setDraftChannelAdFormat } =
		useChannelStore();

	const navigate = useNavigate();

	const onBackButton = () => {
		navigate("/my-channels");
	};

	const onSelectCategory = () => {
		navigate("/select-category");
	};

	const handleSave = () => {
		// Implement save logic here
		console.log("Save button clicked");
	};

	useEffect(() => {
		mainButton.setText("Save");
		mainButton.onClick(handleSave);
		mainButton.show();

		backButton.show();

		backButton.onClick(onBackButton);

		invokeHapticFeedbackImpact("medium");

		return () => {
			mainButton.hide();

			mainButton.offClick(handleSave);

			backButton.hide();

			backButton.offClick(onBackButton);
		};
	}, []);

	const renderSuccess = () => {
		return (
			<div className="Placeholder">
				<div className="Emoji">
					<RLottie sticker="congrats" autoplay width={120} height={120} />
				</div>
				<h2 className="Title">Your Channel Successfully Added</h2>
				<div className="Instructions"></div>
			</div>
		);
	};

	return (
		<div className="SetChannelData">
			<PageHeader>
				<PageHeaderTitle>Create your influence channel</PageHeaderTitle>
			</PageHeader>

			<div className="Section">
				<div className="Items">
					<div className="ChatItem">
						<Avatar id="1" title="Channel 1" photo="" />
						<div className="body">
							<div className="title">Channel 1</div>
							<div className="subtitle">Subtitle 1</div>
						</div>
						<div className="meta">
							<ChevronRight />
						</div>
					</div>
					<div className="Item" onClick={onSelectCategory}>
						<div className="icon">
							<TagIcon />
						</div>
						<div className="body">
							<div className="title">
								{draftChannel?.category?.name || "Select Category..."}
							</div>
						</div>
						<div className="meta">
							<ChevronRight />
						</div>
					</div>
				</div>
			</div>
			<div className="Section Pricing">
				<div className="title">Pricing</div>
				<div className="Items">
					<div className="Item">
						<div className="icon">
							<DollarSignIcon />
						</div>
						<div className="body">
							<div className="price">
								<input type="text" placeholder="Price" />
							</div>
						</div>
						<div className="meta">TON</div>
					</div>
					<Menu
						custom={
							<div className="Item">
								<div className="icon">
									<ClockIcon />
								</div>
								<div className="body">Price type</div>
								<div className="meta">
									{PriceTypes[draftChannel?.priceType] || "Per hour"}
									<ChevronDown />
								</div>
							</div>
						}
					>
						<DropdownMenu className="right">
							{PriceTypes.map((type, index) => (
								<MenuItem
									key={type}
									title={type}
									onClick={() => setDraftChannelPriceType(index)}
								/>
							))}
						</DropdownMenu>
					</Menu>
					<Menu
						custom={
							<div className="Item">
								<div className="icon">
									<SendHorizontalIcon />
								</div>
								<div className="body">Ad format</div>
								<div className="meta">
									{AdFormats[draftChannel?.adFormat] || "Post"}
									<ChevronDown />
								</div>
							</div>
						}
					>
						<DropdownMenu className="right">
							{AdFormats.map((format, index) => (
								<MenuItem
									key={format}
									title={format}
									onClick={() => setDraftChannelAdFormat(index)}
								/>
							))}
						</DropdownMenu>
					</Menu>
					<div className="Item primary">
						<div className="icon">
							<PlusIcon />
						</div>
						<div className="body">
							<div className="title">Add Price...</div>
						</div>
					</div>
				</div>
				<div className="description">Set your price for sponsored content</div>
			</div>
		</div>
	);
}

export default memo(SetChannelData);
