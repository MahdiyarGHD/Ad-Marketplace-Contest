import { memo, useEffect, useState } from "react";
import { DayPicker, type DateRange } from "react-day-picker";
import MainButton from "./MainButton";
import Transition from "./Transition";
import { createPortal } from "react-dom";

function DatePicker({
	show,
	selected,
	title,
	onSelect,
	onClose,
}: {
	show: boolean;
	selected?: DateRange;
	title: string;
	onSelect: (date: DateRange | undefined) => void;
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
					<DayPicker
						disabled={{ before: new Date() }}
						animate
						mode="range"
						selected={selected}
						onSelect={(date) => onSelect(date)}
					/>
					<MainButton text={title} onClick={onClose} />
				</div>
			</Transition>,
			document.body,
		)
	);
}

export default memo(DatePicker);
