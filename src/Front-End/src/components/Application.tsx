import { memo, useState } from "react";
import Menu, { DropdownMenu, MenuItem } from "./Menu";
import TextTransition from "./TextTransition";
import useChannelStore, {
	AdFormats,
	PriceTypes,
	type AdFormat,
	type PriceType,
} from "../stores/useChannelStore";
import {
	CalendarIcon,
	ChevronDown,
	ClockIcon,
	DollarSignIcon,
	SendHorizontalIcon,
} from "lucide-react";
import { useShallow } from "zustand/shallow";
import MainButton from "./MainButton";
import DatePicker from "./DatePicker";
import { Textarea } from "./Textarea";

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

function Application() {
	const [showDatePicker, setShowDatePicker] = useState(false);

	const application = useChannelStore(useShallow((state) => state.application));
	const { setApplication } = useChannelStore(
		useShallow((state) => ({
			setApplication: state.setApplication,
		})),
	);

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

	return (
		<div className="Application">
			<div className="Items">
				<Menu
					custom={({ onClick }) => (
						<div className="Item" onClick={onClick}>
							<div className="icon">
								<ClockIcon />
							</div>
							<div className="body">Proposed Price type</div>
							<div className="meta">
								<TextTransition text={priceType?.title || "Per hour"} />
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
										"Post"
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
							placeholder="Message"
							value={application?.message}
							onChange={(e) => setApplication({ message: e.target.value })}
						/>
					</div>
				</div>
			</div>
			<MainButton text="Submit" onClick={() => {}} />
		</div>
	);
}

export default memo(Application);
