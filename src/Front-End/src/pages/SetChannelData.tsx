import { memo, useEffect } from "react";
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
import useChannelStore, {
	PriceTypes,
	type Channel,
} from "../stores/useChannelStore";
import Menu, { DropdownMenu, MenuItem } from "../components/Menu";
import TextTransition from "../components/TextTransition";
import { requestAPI } from "../utils/api";

const AdFormats = ["Post"];

function SetChannelData() {
	const {
		draftChannel,
		addDraftChannelPrice,
		setDraftChannelPrice,
		setDraftChannelPriceType,
		setDraftChannelAdFormat,
	} = useChannelStore();

	const navigate = useNavigate();

	const onBackButton = () => {
		navigate("/my-channels");
	};

	const onSelectChannel = () => {
		navigate("/select-channel");
	};

	const onSelectCategory = () => {
		navigate("/select-category");
	};

	const handleSave = async () => {
		if (!draftChannel.pricing) return;

		const response = await requestAPI(
			"/api/channels/",
			{
				chat_id: draftChannel.chat_id,
				category_id: draftChannel.category_id,
				pricings: draftChannel.pricing.map((item) => ({
					ad_format: item.ad_format + 1,
					price_type: item.price_type + 1,
					price_ton: item.price_ton,
				})),
			} as Partial<Channel>,
			"POST",
		);

		if (!response.isError) {
			navigate("/add-channel/success");
			invokeHapticFeedbackImpact("medium");
			setTimeout(() => invokeHapticFeedbackImpact("soft"), 200);
			setTimeout(() => invokeHapticFeedbackImpact("soft"), 400);
		}
		// TODO: Handle error with toast
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
	}, [draftChannel]);

	const allPriceTypes = draftChannel.pricing?.map((item) => item.price_type);

	return (
		<div className="SetChannelData scrollable">
			<PageHeader>
				<PageHeaderTitle>Create your influence channel</PageHeaderTitle>
			</PageHeader>

			<div className="Section">
				<div className="Items">
					<div className="ChatItem" onClick={onSelectChannel}>
						<Avatar
							id={draftChannel.chat_id ?? ""}
							title={draftChannel.title ?? "C"} /* photo={draftChannel.photo} */
						/>
						<div className="body">
							<div className="title">
								{draftChannel.title || "Select Channel..."}
							</div>
							{draftChannel.chat_id && (
								<div className="subtitle">{draftChannel.chat_id ?? ""}</div>
							)}
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
				{draftChannel.pricing?.map((item, index) => {
					const availablePriceType = PriceTypes.filter(
						(type, i) =>
							!allPriceTypes!.includes(i) ||
							type === PriceTypes[item.price_type],
					);

					return (
						<div className="Items" key={index}>
							<div className="Item">
								<div className="icon">
									<DollarSignIcon />
								</div>
								<div className="body">
									<div className="price">
										<input
											type="text"
											placeholder="Price"
											value={item.price_ton}
											onChange={(e) =>
												setDraftChannelPrice(Number(e.target.value) || 0, index)
											}
										/>
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
											<TextTransition
												text={PriceTypes[item.price_type] || "Per hour"}
											/>
											<ChevronDown />
										</div>
									</div>
								}
							>
								<DropdownMenu className="right">
									{availablePriceType.map((type) => (
										<MenuItem
											key={type}
											title={type}
											onClick={() =>
												setDraftChannelPriceType(
													PriceTypes.indexOf(type),
													index,
												)
											}
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
											<TextTransition
												text={AdFormats[item.ad_format] || "Post"}
											/>
											<ChevronDown />
										</div>
									</div>
								}
							>
								<DropdownMenu className="right">
									{AdFormats.map((format, i) => (
										<MenuItem
											key={format}
											title={format}
											onClick={() => setDraftChannelAdFormat(i, index)}
										/>
									))}
								</DropdownMenu>
							</Menu>
						</div>
					);
				})}
			</div>
			<div className="Section">
				<div className="Items">
					<div className="Item primary" onClick={() => addDraftChannelPrice()}>
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
