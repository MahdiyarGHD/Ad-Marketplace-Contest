import { memo, useEffect, useState } from "react";
import { DayPicker, type DateRange } from "react-day-picker";
import MainButton from "./MainButton";
import Transition from "./Transition";
import { createPortal } from "react-dom";

function DatePicker({
	show,
	selected,
	title,
	mode = "range",
	onSelect,
	onClose,
}: {
	show: boolean;
	selected?: DateRange | Date;
	title: string;
	mode: "single" | "range";
	onSelect: (date: DateRange | Date | undefined) => void;
	onClose: () => void;
}) {
	const [portal, setPortal] = useState<boolean>(false);

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
							onSelect={onSelect}
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
					<MainButton text={title} onClick={onClose} />
				</div>
			</Transition>,
			document.body,
		)
	);
}

export default memo(DatePicker);
