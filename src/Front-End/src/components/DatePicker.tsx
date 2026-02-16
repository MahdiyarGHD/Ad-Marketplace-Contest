import { memo, useEffect, useState, type ChangeEventHandler } from "react";
import { DayPicker, type DateRange } from "react-day-picker";
import MainButton from "./MainButton";
import Transition from "./Transition";
import { createPortal } from "react-dom";
import { format, setHours, setMinutes } from "date-fns";
import { Clock3Icon } from "lucide-react";

function DatePicker({
	show,
	selected,
	title,
	mode = "range",
	time = false,
	onSelect,
	onClose,
}: {
	show: boolean;
	selected?: DateRange | Date;
	title: string;
	time?: boolean;
	mode: "single" | "range";
	onSelect: (date: DateRange | Date | undefined) => void;
	onClose: () => void;
}) {
	const [portal, setPortal] = useState<boolean>(false);
	const [timeValue, setTimeValue] = useState<string>("00:00");

	useEffect(() => {
		if (!Number.isNaN((selected as Date).getTime())) {
			console.log("are");
			setTimeValue(format(selected as Date, "HH:mm"));
		}
	}, [selected]);

	const handleTimeChange: ChangeEventHandler<HTMLInputElement> = (e) => {
		const time = e.target.value;
		if (!selected) {
			setTimeValue(time);
			return;
		}
		const [hours, minutes] = time.split(":").map((str) => parseInt(str, 10));
		const newSelectedDate = setHours(
			setMinutes(selected as Date, minutes),
			hours,
		);
		onSelect(newSelectedDate);
		setTimeValue(time);
	};

	const handleDaySelect = (date: Date | undefined) => {
		if (!timeValue || !date) {
			onSelect(date);
			return;
		}
		const [hours, minutes] = timeValue
			.split(":")
			.map((str) => parseInt(str, 10));
		const newDate = new Date(
			date.getFullYear(),
			date.getMonth(),
			date.getDate(),
			hours,
			minutes,
		);
		onSelect(newDate);
	};

	useEffect(() => {
		if (show) setPortal(true);
	}, [show]);

	return (
		portal &&
		createPortal(
			<Transition
				state={show}
				eachElement
				onDeactivate={() => {
					setPortal(false);
				}}
			>
				<div className="bg" onClick={onClose}></div>
				<div className="DatePickerWrapper">
					{mode === "single" ? (
						<DayPicker
							disabled={{ before: new Date() }}
							animate
							mode="single"
							selected={selected as Date}
							onSelect={time ? handleDaySelect : onSelect}
						/>
					) : (
						<DayPicker
							disabled={{ before: new Date() }}
							animate
							mode="range"
							selected={selected as DateRange}
							onSelect={onSelect}
						/>
					)}
					{time && (
						<div className="Items">
							<div className="Item">
								<div className="icon">
									<Clock3Icon />
								</div>
								<div className="body">
									<div className="title">Set the time</div>
								</div>
								<input
									type="time"
									value={timeValue}
									onChange={handleTimeChange}
								/>
							</div>
						</div>
					)}
					<MainButton text={title} onClick={onClose} />
				</div>
			</Transition>,
			document.body,
		)
	);
}

export default memo(DatePicker);
