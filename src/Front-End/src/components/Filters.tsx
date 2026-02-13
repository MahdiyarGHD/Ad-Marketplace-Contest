import { memo } from "react";
import { AdFormats, PriceTypes } from "../stores/useChannelStore";
import Menu, { DropdownMenu, MenuItem } from "./Menu";
import TextTransition from "./TextTransition";
import { ChevronDown, ChevronRight } from "lucide-react";
import MainButton from "./MainButton";
import type { Filter, OptionsFilter } from "../stores/useUIStore";
import useUIStore from "../stores/useUIStore";
import { useShallow } from "zustand/shallow";
import { useNavigate } from "react-router-dom";
import useCategoryStore from "../stores/useCategoryStore";

function Filters() {
	const values = useUIStore(useShallow((state) => state.search?.filters));
	const setFilter = useUIStore(useShallow((state) => state.search?.setFilter));

	const { getCategory } = useCategoryStore();

	const navigate = useNavigate();

	const filters: Filter[] = [
		{
			key: "category",
			title: "Category",
			type: "text",
			onClick: () => navigate("/select-category/filter"),
		},
		{
			key: "subscribers",
			title: "Subscribers",
			type: "range",
			range: [0, 1_000_000],
		},
		{
			key: "avgViews",
			title: "Min Average Views",
			type: "number",
			unit: "Views",
			range: [0, 1_000_000],
		},
		{
			key: "MinPrice",
			title: "Min Price",
			type: "number",
			unit: "TON",
			range: [0, 100_000],
		},
		{
			key: "adFormat",
			title: "Ad Format",
			type: "options",
			options: AdFormats,
		},
		{
			key: "priceType",
			title: "Price Type",
			type: "options",
			options: PriceTypes,
		},
	];

	console.log("Filters", values);

	const renderFilterWithOptions = (filter: OptionsFilter) => {
		return (
			<Menu
				custom={({ onClick }) => (
					<div className="Item" onClick={onClick}>
						{/* <div className="icon"></div> */}
						<div className="body">{filter.title}</div>
						<div className="meta">
							<TextTransition
								text={
									filter.options[values?.[filter.key] as string] || "Select"
								}
							/>
							<ChevronDown />
						</div>
					</div>
				)}
			>
				<DropdownMenu className="right">
					{Object.entries(filter.options).map(([key, type]) => (
						<MenuItem
							key={key}
							title={type}
							onClick={() => setFilter?.(filter.key, key)}
						/>
					))}
				</DropdownMenu>
			</Menu>
		);
	};

	const renderFilters = (filter: Filter) => {
		switch (filter.type) {
			case "text":
				return (
					<div className="Item" key={filter.title} onClick={filter.onClick}>
						{/* <div className="icon"></div> */}
						<div className="body">
							<div className="title">{filter.title}</div>
						</div>
						<div className="meta">
							{(filter.key === "category" &&
								values?.[filter.key] &&
								getCategory(values?.[filter.key] as string)?.name) ||
								"Select"}
							<ChevronRight />
						</div>
					</div>
				);
			case "number":
				return (
					<div className="Item" key={filter.title}>
						{/* <div className="icon"></div> */}
						<div className="body">
							<div className="title">{filter.title}</div>
						</div>
						<div className="meta">
							<input
								type="number"
								placeholder="0"
								value={(values?.[filter.key] as number) || ""}
								onChange={(e) =>
									setFilter?.(filter.key, Number(e.target.value))
								}
							/>
							{filter.unit}
						</div>
					</div>
				);
			case "range":
				return (
					<div className="Item" key={filter.title}>
						{/* <div className="icon"></div> */}
						<div className="body multiline">
							<div className="title">{filter.title}</div>
							<div className="flex">
								<input
									type="number"
									placeholder="Min"
									value={(values?.[filter.key] as number[])?.[0] || ""}
									onChange={(e) =>
										setFilter?.(filter.key, [
											Math.max(filter.range[0], Number(e.target.value)),
											(values?.[filter.key] as number[])?.[1] || 0,
										])
									}
								/>
								<input
									type="number"
									placeholder="Max"
									value={(values?.[filter.key] as number[])?.[1] || ""}
									onChange={(e) =>
										setFilter?.(filter.key, [
											(values?.[filter.key] as number[])?.[0] || 0,
											Math.min(Number(e.target.value), filter.range[1]),
										])
									}
								/>
							</div>
						</div>
					</div>
				);
			case "options":
				return renderFilterWithOptions(filter);
			default:
				return null;
		}
	};

	return (
		<div className="Filters">
			{filters.map((filter) => (
				<div className="Items" key={filter.key}>
					{renderFilters(filter)}
				</div>
			))}
			<MainButton text="Apply" onClick={() => {}} />
		</div>
	);
}

export default memo(Filters);
