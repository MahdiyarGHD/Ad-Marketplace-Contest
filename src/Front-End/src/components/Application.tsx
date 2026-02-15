import { memo, useEffect, useState } from "react";
import Menu, { DropdownMenu, MenuItem } from "./Menu";
import TextTransition from "./TextTransition";
import {
	AdFormats,
	PriceTypes,
	type AdFormat,
	type PriceType,
} from "../stores/useChannelStore";
import {
	CalendarIcon,
	ChevronDown,
	ChevronRight,
	ClockIcon,
	DollarSignIcon,
	SendHorizontalIcon,
} from "lucide-react";
import { useShallow } from "zustand/shallow";
import MainButton from "./MainButton";
import DatePicker from "./DatePicker";
import { Textarea } from "./Textarea";
import { requestAPI } from "../utils/api";
import { invokeHapticFeedbackImpact } from "../utils/common";
import useUIStore from "../stores/useUIStore";
import RLottie from "./RLottie";
import useApplicationStore, {
	type ApplicationType,
} from "../stores/useApplicationStore";
import Avatar from "./Avatar";
import { useNavigate } from "react-router-dom";

export const PriceTypesInfo = {
	1: {
		title: "Per hour",
		unit: "hours",
		max: 10000,
	},
	2: {
		title: "Per day",
		unit: "days",
		max: 365,
	},
	3: {
		title: "Per 1000 views",
		unit: "views",
		max: 100000000,
	},
};

function Application({
	onClose,
	type,
	counterOffer = false,
}: {
	onClose: () => void;
	type: "channel" | "campaign";
	counterOffer?: boolean;
}) {
	const [showDatePicker, setShowDatePicker] = useState(false);
	const [success, setSuccess] = useState(false);

	const application = useApplicationStore(
		useShallow((state) => state.application),
	);
	const setApplication = useApplicationStore(
		useShallow((state) => state.setApplication),
	);

	const showToast = useUIStore(useShallow((state) => state.showToast));

	const navigate = useNavigate();

	const availablePriceType = Object.entries(PriceTypes).filter(([key]) =>
		application?.channel?.pricings?.find((p) => p.price_type === Number(key)),
	);

	const priceType =
		PriceTypesInfo[application?.proposed_price_type as PriceType];

	const calculateTotal = () => {
		if (!application?.proposed_value || !application?.channel?.pricings)
			return 0;

		const pricing = application.channel.pricings.find(
			(p) => p.price_type === application.proposed_price_type,
		);
		if (!pricing) return 0;

		if (application.proposed_value > priceType?.max) {
			setApplication({ proposed_value: priceType?.max });
			return priceType?.max * pricing.price_ton;
		}

		if (application.proposed_price_type === 3) {
			return (application.proposed_value * pricing.price_ton) / 1000;
		}

		return application.proposed_value * pricing.price_ton;
	};

	const onSelectChannel = () => {
		navigate("/select-my-channel/application");
	};

	const handleApply = async () => {
		try {
			if (!application?.channel_id) {
				throw new Error("Please select a channel");
			}
			if (!application?.proposed_value) {
				throw new Error("Please enter a proposed value");
			}
			if (application?.proposed_value && application?.proposed_value <= 0) {
				throw new Error("Proposed value must be greater than 0");
			}
			if (!application?.proposed_price_type) {
				throw new Error("Please select a proposed price type");
			}
			if (!application.proposed_posting_time) {
				throw new Error("Please select a proposed posting time");
			}
			if (!application?.proposed_ad_format) {
				throw new Error("Please select a proposed ad format");
			}
			if (!application?.message) {
				throw new Error("Please enter a message");
			}
		} catch (error) {
			showToast({ title: (error as Error).message });
			return;
		}
		const endpoint =
			type === "channel" ? "channel-applications" : "applications";

		const response = await requestAPI(
			`/api/${endpoint}/${counterOffer ? `${application?.id}/counter-offer` : ""}`,
			{
				campaign_id:
					type === "campaign" && !counterOffer && application?.campaign_id,
				channel_id: !counterOffer && application?.channel_id,
				message: application?.message,
				proposed_ad_format: application?.proposed_ad_format,
				proposed_price_type: application?.proposed_price_type,
				proposed_price_ton: calculateTotal(),
				proposed_posting_time: application?.proposed_posting_time,
			} as Partial<ApplicationType>,
			"POST",
		);

		console.log(application);

		if (!response.isError && response.value) {
			// navigate(isUpdating ? `/my-campaigns` : "/add-campaign/success");
			invokeHapticFeedbackImpact("medium");
			setTimeout(() => invokeHapticFeedbackImpact("soft"), 200);
			setTimeout(() => invokeHapticFeedbackImpact("soft"), 300);
			setSuccess(true);
			// clearDraftCampaign();
		} else {
			showToast({ title: response.first_error.description });
		}
	};

	const getChannelInfo = async () => {
		if (!application?.channel_id) return;

		const response = await requestAPI(
			`/api/channels/${application.channel_id}`,
			{},
			"GET",
		);

		if (!response.isError && response.value) {
			setApplication({ channel: response.value });
		}
	};

	useEffect(() => {
		invokeHapticFeedbackImpact("medium");
	}, []);

	useEffect(() => {
		if (!application?.channel && application?.channel_id) {
			getChannelInfo();
		}
	}, [application?.channel_id]);

	const renderSuccess = () => {
		return (
			<div className="Application">
				<div className="Placeholder">
					<div className="Emoji">
						<RLottie sticker="submitted" autoplay width={120} height={120} />
					</div>
					<h2 className="Title">Your Application Submitted</h2>
					<div className="Subtitle">
						Your application has been successfully submitted.
						<br />
					</div>
				</div>
				<MainButton text="Done" onClick={onClose} />
			</div>
		);
	};

	if (success) {
		return renderSuccess();
	}

	return (
		<div className="Application">
			{type === "campaign" && !counterOffer && (
				<div className="Items">
					<div className="ChatItem" onClick={onSelectChannel}>
						<Avatar
							id={application?.channel?.chat_id ?? ""}
							title={
								application?.channel?.title ?? "C"
							} /* photo={draftChannel.photo} */
						/>
						<div className="body">
							<div className="title">
								{application?.channel?.title || "Select Channel..."}
							</div>
							{application?.channel?.chat_id && (
								<div className="subtitle">
									{application?.channel?.username ?? "private channel"}
								</div>
							)}
						</div>
						<div className="meta">
							<ChevronRight />
						</div>
					</div>
				</div>
			)}
			<div className="Items">
				<Menu
					custom={({ onClick }) => (
						<div className="Item" onClick={onClick}>
							<div className="icon">
								<ClockIcon />
							</div>
							<div className="body">Proposed Price type</div>
							<div className="meta">
								<TextTransition text={priceType?.title || "None"} />
								<ChevronDown />
							</div>
						</div>
					)}
				>
					<DropdownMenu className="right">
						{availablePriceType.map(([key, type]) => (
							<MenuItem
								key={key}
								title={type}
								onClick={() =>
									setApplication({ proposed_price_type: Number(key) })
								}
							/>
						))}
					</DropdownMenu>
				</Menu>
				<Menu
					custom={({ onClick }) => (
						<div className="Item" onClick={onClick}>
							<div className="icon">
								<SendHorizontalIcon />
							</div>
							<div className="body">Proposed Ad format</div>
							<div className="meta">
								<TextTransition
									text={
										AdFormats[application?.proposed_ad_format as AdFormat] ||
										"None"
									}
								/>
								<ChevronDown />
							</div>
						</div>
					)}
				>
					<DropdownMenu className="right">
						{Object.entries(AdFormats).map(([key, format]) => (
							<MenuItem
								key={format}
								title={format}
								onClick={() =>
									setApplication({ proposed_ad_format: Number(key) })
								}
							/>
						))}
					</DropdownMenu>
				</Menu>
				<div className="Item">
					<div className="icon">
						<DollarSignIcon />
					</div>
					<div className="body">
						<div className="flex">
							<input
								type="number"
								placeholder="0"
								value={application?.proposed_value || ""}
								onChange={(e) =>
									setApplication({ proposed_value: Number(e.target.value) })
								}
							/>
						</div>
						<div className="subtitle">{priceType?.unit || "hours"}</div>
					</div>
					<div className="meta">
						{application?.proposed_value && application?.channel?.pricings
							? `${calculateTotal()} TON`
							: "0 TON"}
					</div>
				</div>
				<div className="Item" onClick={() => setShowDatePicker(true)}>
					<div className="icon">
						<CalendarIcon />
					</div>
					<div className="body">Proposed Posting time</div>
					<div className="meta">
						<TextTransition
							text={
								application?.proposed_posting_time
									? new Date(
											application?.proposed_posting_time,
										).toLocaleDateString()
									: "Select date"
							}
						/>
					</div>
				</div>
				<DatePicker
					show={showDatePicker}
					title="Select Date"
					mode="single"
					selected={new Date(application?.proposed_posting_time || "")}
					onSelect={(date) =>
						setApplication({
							proposed_posting_time: date
								? new Date(date as Date).toISOString()
								: undefined,
						})
					}
					onClose={() => setShowDatePicker(false)}
				/>
				<div className="Item">
					<div className="body">
						<Textarea
							style={{ minHeight: 64 }}
							placeholder="Message"
							value={application?.message}
							onChange={(e) => setApplication({ message: e.target.value })}
						/>
					</div>
				</div>
			</div>
			<MainButton text="Submit" onClick={handleApply} />
		</div>
	);
}

export default memo(Application);
